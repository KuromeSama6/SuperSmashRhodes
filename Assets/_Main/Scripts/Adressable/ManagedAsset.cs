using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SuperSmashRhodes.Adressable {
public class ManagedAsset {
    public AssetStatus status { get; private set; }
    public AssetReleaseMethod releaseMethod = AssetReleaseMethod.ON_SCENE_UNLOAD;
    private AsyncOperationHandle<object> handle;
    private string key;
    
    public ManagedAsset(string key) {
        this.key = key;
        status = AssetStatus.NOT_LOADED;

    }

    public void Load(Action callback = null) {
        if (status == AssetStatus.LOADING || status == AssetStatus.STANDBY)
            Release();

        handle = Addressables.LoadAssetAsync<object>(key);
        status = AssetStatus.LOADING;
        handle.Completed += res => {
            if (res.Status == AsyncOperationStatus.Succeeded) {
                status = AssetStatus.STANDBY;
                if (callback != null) {
                    callback.Invoke();
                }
                
                // Debug.Log($"{key} loaded");
                
            } else {
                Debug.LogError($"Failed to load asset {key}, error: {res.OperationException}");
                Release();
            }
        };
    }

    public void Get<T>(Action<T> callback) {
        if (status == AssetStatus.STANDBY) {
            callback.Invoke((T)handle.Result);
        } else {
            Load(() => callback.Invoke((T)handle.Result));
        }
    }

    public T GetNow<T>() {
        if (status == AssetStatus.STANDBY) return (T)handle.Result;
        return default;
    }
    
    public void Release() {
        if (!handle.IsValid()) return;
        if (status == AssetStatus.RELEASED) {
            return;
        }
        handle.Release();
        status = AssetStatus.RELEASED;
    }
    
}

public enum AssetStatus {
    NOT_LOADED,
    LOADING,
    STANDBY,
    RELEASED
}

public enum AssetReleaseMethod {
    MANUAL,
    ON_SCENE_UNLOAD,
}
}
