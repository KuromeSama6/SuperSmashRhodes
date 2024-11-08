using SuperSmashRhodes.FScript.Components;
using UnityEngine;

namespace SuperSmashRhodes.FScript.Function.Library {
[FFunctionLibrary]
public static class PhysicsFuncs {
    [FFunction]
    public static void AddCarriedForce(FScriptRuntime ctx, FImmediate x, FImmediate y) {
        ctx.owner.AddCarriedForce(new Vector2(x.FloatValue(ctx), y.FloatValue(ctx)));
    }
}
}
