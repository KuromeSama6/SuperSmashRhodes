using System;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Battle.FX;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Battle {
public class CharacterRenderController : MonoBehaviour {
    private static readonly int RENDER_COLOR_WHITE = Shader.PropertyToID("_Color");
    private static readonly int RENDER_COLOR_BLACK = Shader.PropertyToID("_Black");
    
    [Title("References")]
    public Renderer renderer;

    private PlayerCharacter owner;
    private CharacterFXManager fxManager;

    private void Start() {
        owner = GetComponent<PlayerCharacter>();
        fxManager = owner.fxManager;
    }

    private void FixedUpdate() {
        var mat = renderer.material;

        if (!fxManager.playFlash) {
            if (owner.activeState == null) return;
            var data = owner.activeState.stateData.renderColorData;
            if (!data.flags.HasFlag(CharacterRenderColorData.Flag.PAUSE)) {
                mat.SetColor(RENDER_COLOR_WHITE, Color.Lerp(mat.GetColor(RENDER_COLOR_WHITE), data.white, data.lerpSpeed * Time.fixedDeltaTime));

                if (mat.HasColor(RENDER_COLOR_BLACK)) {
                    mat.SetColor(RENDER_COLOR_BLACK, Color.Lerp(mat.GetColor(RENDER_COLOR_BLACK), data.black, data.lerpSpeed * Time.fixedDeltaTime));      
                }
            }
            
        } else {
            var flash = Time.frameCount % 8 < 4;
            if (mat.HasColor(RENDER_COLOR_BLACK)) {
                mat.SetColor(RENDER_COLOR_BLACK, Color.Lerp(Color.black, Color.white, flash ? .5f : 0f));
            }
        }
        
        renderer.material = mat; 
    }

}
}
