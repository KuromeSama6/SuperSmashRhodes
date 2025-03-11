using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DotNet.Globbing;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Util;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace SuperSmashRhodes.Adressable {
public class AssetManager : AutoInitSingletonBehaviour<AssetManager> {
    private Dictionary<string, ManagedAsset> assets = new();
    private List<Action> queue = new();
    private bool ready;
    
    private IEnumerator Start() {
        var handle = Addressables.InitializeAsync();
        yield return handle;
        
        foreach (var locator in Addressables.ResourceLocators) {
            foreach (var key in locator.Keys) {
                assets[key.ToString()] = new(key.ToString());
            }
        }
        
        // Debug.Log($"loaded {assets.Count} addressables");
        ready = true;
        queue.ForEach(c => c.Invoke());

        SceneManager.sceneUnloaded += OnSceneUnloaded;
        
        InstantiateAutoPrefabs();
    }

    public List<string> GetAssets(string pattern) {
        var glob = Glob.Parse(pattern);
        return assets.Keys.Where(c => glob.IsMatch(c)).ToList();
    }
    
    public void PreloadAll(string pattern, AssetReleaseMethod releaseMethod = AssetReleaseMethod.MANUAL) {
        if (!ready) {
            queue.Add(() => PreloadAll(pattern, releaseMethod));
            return;
        }
        
        var glob = Glob.Parse(pattern);
        // List<string> toLoad = new();
        
        foreach (var key in assets.Keys) {
            if (assets.ContainsKey(key) && assets[key].status == AssetStatus.STANDBY) {
                continue;
            }

            if (glob.IsMatch(key)) {
                assets[key].releaseMethod = releaseMethod;
                assets[key].Load();
            }
        }
    }
    
    private void OnSceneUnloaded(Scene scene) {
        foreach (var asset in assets.Values) {
            if (asset.releaseMethod == AssetReleaseMethod.ON_SCENE_UNLOAD) {
                // asset.Release();
            }
        }
    }
    
    
    private void InstantiateAutoPrefabs() {
        foreach (var path in GetAssets("__auto/**")) {
            Get<GameObject>(path, res => {
                var go = Instantiate(res);
                go.name = $"{res.name}$Auto";
                DontDestroyOnLoad(go);
            }, AssetReleaseMethod.MANUAL);
        }
    }

    private void OnDestroy() {
        // release all
        foreach (var asset in assets.Values) {
            asset.Release();
        }
    }

    public static T Get<T>(AssetReferenceT<T> reference) where T: Object {
        return Get<T>(reference.RuntimeKey.ToString());
    }
    
    public static T Get<T>(string key) where T: Object {
        if (inst.assets.ContainsKey(key)) {
            return inst.assets[key].GetNow<T>();
            
        } else {
            Debug.LogError($"Asset {key} not found");
            return null;
        }
    }
    
    public static void Get<T>(AssetReferenceT<T> reference, Action<T> callback, AssetReleaseMethod releaseMethod = AssetReleaseMethod.ON_SCENE_UNLOAD) where T: Object {
        Get(reference.RuntimeKey.ToString(), callback, releaseMethod);
    }
    
    public static void Get<T>(string key, Action<T> callback, AssetReleaseMethod releaseMethod = AssetReleaseMethod.ON_SCENE_UNLOAD) where T: Object {
        if (!inst.ready) {
            inst.queue.Add(() => Get(key, callback));
            return;
        }
        
        if (inst.assets.ContainsKey(key)) {
            inst.assets[key].Get(callback, releaseMethod);
            
        } else {
            Debug.LogError($"Asset {key} not found");
        }
    }
    
    
}
}
