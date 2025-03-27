using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Scripts.Audio;
using UnityEngine;

namespace SuperSmashRhodes.UI.Toast {
public class ToastManager : SingletonBehaviour<ToastManager> {
    [Title("References")]
    public RectTransform container;
    public GameObject toastPrefab;

    private void Start() {
        foreach (RectTransform go in container) Destroy(go.gameObject);
    }

    private void Update() {
        
    }

    public void ShowToast(ToastType type, string title, string content, float duration = 3f) {
        var toast = Instantiate(toastPrefab, container).GetComponent<Toast>();
        toast.Init(type, title, content, duration);
        AudioManager.inst.PlayAudioClip("cmn/sfx/ui/prompt/normal", gameObject);
    }
    
}

}
