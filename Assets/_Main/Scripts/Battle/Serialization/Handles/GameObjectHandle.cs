using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Battle.Serialization {
public struct GameObjectHandle : IHandle { 
    private string path;
    private Vector3 position;
    private Quaternion rotation;
    private Vector3 scale;
    
    public GameObjectHandle(GameObject obj) {
        path = obj.GetAbsolutePath();
        position = obj.transform.position;
        rotation = obj.transform.rotation;
        scale = obj.transform.localScale;
    }
    
    public object Resolve() {
        var ret =  GameObjectUtil.FindOrCreateByAbsolutePath(path);
        ret.transform.position = position;
        ret.transform.rotation = rotation;
        ret.transform.localScale = scale;
        return ret;
    }

    public override string ToString() {
        return $"GameObjectHandle(path={path}, position={position}, rotation={rotation}, scale={scale})";
    }
}
}
