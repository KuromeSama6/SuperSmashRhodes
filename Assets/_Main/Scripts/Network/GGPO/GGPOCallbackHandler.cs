using SuperSmashRhodes.Battle.Serialization;
using SuperSmashRhodes.GGPOWrapper.Packet;
using SuperSmashRhodes.Network.Rollbit.P2P;
using SuperSmashRhodes.Util;

namespace SuperSmashRhodes.GGPOWrapper {
public interface IConnectorCallbackHandler {
    void OnRollbackTickFrame();
    void OnLoadGameState(int handle);
    bool OnSaveGameState(int handle);
    void OnDeleteSavedGameState(int handle);
    void OnNetworkStatusChanged(GGPOConnectionStatus newStatus, GGPOConnectionStatus oldStatus);
    void OnReceivedAuxiliaryData(ChannelSubpacketCustom subpacket);
}
}
