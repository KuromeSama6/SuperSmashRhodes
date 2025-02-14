using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace SuperSmashRhodes.FX {
public class FlipbookPlayer : MonoBehaviour {
    [Title("References")]
    public FlipbookData flipbookData;
    public bool loop;
    
    private SpriteRenderer renderer;
    
    public bool playing { get; private set; }
    public bool finished { get; private set; }
    public bool destroyOnFinish { get; set; } = true;

    private float counter;
    
    public int frame {
        get => _frame;
        set {
            _frame = value;
            try {
                renderer.sprite = flipbookData.sprites[_frame];
            } catch {
                
            }
        }
    }
    private int _frame;

    public int totalFrames => Math.Min(flipbookData.frames, flipbookData.sprites.Count);
    
    public void Start() {
        renderer = GetComponentInChildren<SpriteRenderer>() ?? gameObject.AddComponent<SpriteRenderer>();
        Play();
    }

    private void Update() {
        if (finished) {
            if (destroyOnFinish) Destroy(gameObject);
            return;
        }

        if (!playing) return;
        if (counter == 0) {
            if (frame < totalFrames - 1) {
                ++frame;
            } else {
                finished = true;
            }
            
            renderer.sprite = flipbookData.sprites[frame];
        }
        
        counter += Time.deltaTime;
        if (counter >= 1f / flipbookData.frameRate) {
            counter = 0;
        }
    }

    public void Play() {
        playing = true;
        finished = false;
        frame = 0;
    }

}
}
