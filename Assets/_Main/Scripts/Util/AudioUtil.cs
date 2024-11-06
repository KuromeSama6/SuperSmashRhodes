using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace SuperSmashRhodes.Util {
public static class AudioUtil {
    public static IEnumerator GetAudioClipWebCoroutine(string path, Action<AudioClip> callback, AudioType type = AudioType.WAV) {
        string pth = ConvertToFileUri(path);
        using (var www = UnityWebRequestMultimedia.GetAudioClip(pth, type)) {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success) {
                Debug.LogError(www.error);
                callback(null);
            } else {
                var clip = DownloadHandlerAudioClip.GetContent(www);
                clip.LoadAudioData();
                callback(clip);
            }
        }
    }
    
    public static string ConvertToFileUri(string windowsPath) {
        // Get the full absolute path
        string fullPath = Path.GetFullPath(windowsPath);

        // Replace backslashes with forward slashes
        string uriPath = fullPath.Replace('\\', '/');

        // Add the file:// prefix
        return "file:///" + uriPath;
    }

    public struct AudioData {
        public int sampleLength;
        public int channels;
        public int sampleRate;
        public float[] samples;
    }
}
}
