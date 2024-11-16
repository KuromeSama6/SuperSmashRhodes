using System;
using UnityEngine.AddressableAssets;

namespace SuperSmashRhodes.Util {
public static class AddressablesUtil {
    public static void LoadAsync<T>(string name, Action<T> callback) {
        var op = Addressables.LoadAssetAsync<T>(name);
        op.Completed += handle => {
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded) {
                var asset = handle.Result;
                callback(asset);
                op.Release();
            }
        };
    }
}
}
