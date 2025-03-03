using System;
using System.Collections.Generic;
using System.Linq;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Network.Rollbit;
using SuperSmashRhodes.Network.Rollbit.P2P;
using SuperSmashRhodes.Network.RoomManagement;
using UnityEngine;

namespace SuperSmashRhodes.Network {
public class NetworkInputManager {
    public int sendFrame { get; private set; }
    public int receiveFrame { get; private set; }
    public int localReceiveFrame { get; private set; }
    
    public bool active { get; set; }
    
    private readonly NetworkRoom room;
    private readonly Dictionary<int, NetworkInputFrame> receiveQueue = new();
    private readonly Dictionary<int, CachedInputFrame> inputCache = new();
    
    private P2PConnector p2PConnector => room.p2PConnector;
    private NetcodeMode netcodeMode => room.session.config.netcodeMode;
    
    // public bool rollbackMode { get; private set; }
    // public int rollbackModeFrame { get; private set; }
    // public int rollbackTargetFrame { get; private set; }
    
    public NetworkInputBufferWrapper remoteBuffer { get; private set; } = new();
    
    public bool pauseGameState {
        get {
            if (!active) return false;
            
            if (netcodeMode == NetcodeMode.ROLLBACK) {
                return sendFrame - room.session.config.maxRollbackFrames > receiveFrame; 
                
            } else {
                return sendFrame > receiveFrame;
            }
        }
    }

    public bool shouldCacheGameState {
        get {
            if (!active) return false;
            return netcodeMode == NetcodeMode.ROLLBACK;
        }
    }
    
    public NetworkInputManager(NetworkRoom room) {
        this.room = room;
    }

    public void AddInput(NetworkInputFrame frame) {
        if (!active) return;

        if (frame.frame - receiveFrame > 60) {
            Debug.LogWarning($"Received frame {frame.frame} is too far ahead of current frame {receiveFrame}, ignoring");
            return;
        }
        
        if (receiveQueue.ContainsKey(frame.frame)) {
            throw new ArgumentException($"Frame {frame.frame} already exists in the receive queue. Current frame: {receiveFrame}");
        }
        
        if (frame.frame <= receiveFrame) {
            throw new ArgumentException($"Received frame {frame.frame} is older than current frame {receiveFrame}");
        }
        
        receiveQueue.Add(frame.frame, frame);
        
        Debug.Log($"Received actual frame {frame.frame}, current frame {receiveFrame}, send frame {sendFrame}, queue {string.Join(", ", receiveQueue.Keys)}, data {frame}");
        ApplyRemoteInput();
        
    }

    private void ApplyRemoteInput() {
        if (receiveQueue.ContainsKey(receiveFrame + 1)) {
            if (netcodeMode == NetcodeMode.DELAY) {
                ++localReceiveFrame;
                ++receiveFrame;
                
                var inputFrame = receiveQueue[receiveFrame];
                remoteBuffer.inputBuffer.PushAndTick(inputFrame.inputs);
                receiveQueue.Remove(receiveFrame);
                
            } else {
                // rollback inputs are applied on the next tick
            }
        }
    }
    
    /// <summary>
    /// Ticks this input manager and sends the current frame to the other player.
    /// This should be called after the frame update.
    /// </summary>
    public void Tick() {
        if (!active) return;
        Debug.Log($"--- New Tick: Send {sendFrame}, Receive {receiveFrame}, Local Receive {localReceiveFrame}, Check Frame {receiveFrame + 1}  ---");
        // check for new inputs in queue
        bool doRollback = false;
        if (netcodeMode == NetcodeMode.ROLLBACK && receiveQueue.Count > 0) {
            var frame = receiveFrame + 1;
            if (receiveQueue.ContainsKey(frame) && inputCache.ContainsKey(frame)) {
                // compare
                var cached = inputCache[frame].predictedRemoteInputs;
                var actual = receiveQueue[frame].inputs;
                // compare ignore order
                var equal = CompareInputs(cached, actual);
                
                if (equal) {
                    Debug.Log($"Prediction was correct for remote input frame {frame}");
                    ++receiveFrame;
                    GameStateManager.inst.DeleteGameState(frame);
                    receiveQueue.Remove(frame);

                } else {
                    Debug.LogWarning($"Cached input for frame {frame} does not match actual input. Cached: {string.Join(", ", cached)}, actual: {string.Join(", ", actual)}");
                    doRollback = true;
                }
            }
        }
        
        if (doRollback) {
            Debug.Log("beginning rollback");
            var start = receiveFrame + 1;
            GameStateManager.inst.RollbackToFrame(start);
            
            remoteBuffer.SetBuffer(inputCache[start].remoteBuffer);
            
            for (int frame = start; frame <= localReceiveFrame; frame++) {
                Debug.Log($"Rollback frame #{frame}, receive queue contains key: {receiveQueue.ContainsKey(frame)}");
                var cached = inputCache[frame];
                
                room.localPlayer.playerCharacter.inputProvider.SetBuffer(cached.localBuffer);
                if (receiveQueue.ContainsKey(frame)) {
                    remoteBuffer.inputBuffer.PushAndTick(receiveQueue[frame].inputs);
                    if (frame == start) ++receiveFrame;
                    receiveQueue.Remove(frame);
                    Debug.Log($"Solified frame {frame}");
                    
                } else {
                    // repredict based on new input
                    var predictedRemote = PredictInput(remoteBuffer.inputBuffer.thisFrame.inputs);
                    remoteBuffer.inputBuffer.PushAndTick(predictedRemote);
                    
                    inputCache[frame] = new(frame, cached.localBuffer, remoteBuffer.inputBuffer, predictedRemote);
                    Debug.Log($"repredicted frame {frame}");
                }
                
                GameStateManager.inst.TickGameStateImmediate();
            }
            
        }
        
        // poll local input
        var character = room.localPlayer.playerCharacter;
        if (!character) return;
        var chord = character.inputProvider.inputBuffer.thisFrame;

        ++sendFrame;
        var arr = chord.inputs.ToArray();
        p2PConnector.SendInput(sendFrame, arr);
        
        if (netcodeMode == NetcodeMode.ROLLBACK) {
            InputFrame[] remote;
            bool predicted = true;
            if (receiveQueue.ContainsKey(sendFrame)) {
                Debug.Log($"remote input for frame {sendFrame} already exists");
                remote = receiveQueue[sendFrame].inputs;
                predicted = false;
                // receiveQueue.Remove(sendFrame);
                // receiveFrame = Math.Max(receiveFrame, sendFrame);

            } else {
                Debug.Log($"No remote input for frame {sendFrame}, predicting");
                remote = PredictInput(remoteBuffer.inputBuffer.thisFrame.inputs);
            }
            
            remoteBuffer.inputBuffer.PushAndTick(remote);

            
            inputCache[sendFrame] = new(sendFrame, room.localPlayer.playerCharacter.inputProvider.inputBuffer, remoteBuffer.inputBuffer, remote);
            
            ++localReceiveFrame;
        }
        
        Debug.Log($"send frame {sendFrame}, rcv {receiveFrame}, local rcv {localReceiveFrame} inputs {string.Join(", ", chord.inputs)}");    
        
    }

    private InputFrame[] PredictInput(InputFrame[] frame) {
        // simple prediction
        //TODO: thresholds for different input types
        return frame.ToArray();
    }
    
    public void Reset() {
        receiveFrame = 0;
        sendFrame = 0;
        receiveQueue.Clear();
        inputCache.Clear();
    }

    private static bool CompareInputs(InputFrame[] a, InputFrame[] b) {
        if (a.Length != b.Length) return false;
        for (int i = 0; i < a.Length; i++) {
            if (!a[i].Equals(b[i])) return false;
        }
        return true;
    }
}

public struct NetworkInputFrame {
    public readonly int frame;
    public readonly InputFrame[] inputs;

    public NetworkInputFrame(int frame, InputFrame[] inputs) {
        this.frame = frame;
        this.inputs = inputs;
    }

    public override string ToString() {
        return $"NetworkInputFrame(frame={frame} inputs=[{string.Join(", ", inputs.Select(i => i.ToString()))}])";
    }
}

struct CachedInputFrame {
    public readonly int frame;
    public readonly InputBuffer localBuffer;
    public readonly InputBuffer remoteBuffer;
    public readonly InputFrame[] predictedRemoteInputs;
    
    public CachedInputFrame(int frame, InputBuffer local, InputBuffer remote, InputFrame[] predictedRemoteInputs) {
        this.frame = frame;
        localBuffer = local.Copy();
        remoteBuffer = remote.Copy();
        this.predictedRemoteInputs = predictedRemoteInputs;
    }
}
}
