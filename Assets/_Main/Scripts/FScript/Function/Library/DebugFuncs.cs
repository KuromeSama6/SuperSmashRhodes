using System.Linq;
using SuperSmashRhodes.FScript.Components;
using UnityEngine;

namespace SuperSmashRhodes.FScript.Function.Library {
[FFunctionLibrary]
public class DebugFuncs {
    [FFunction]
    public static void Log(FScriptRuntimeContext ctx, params FImmediate[] message) {
        Debug.Log(string.Join(" ", message.Select(c => c.StringValue(ctx))));
    }
}
}
