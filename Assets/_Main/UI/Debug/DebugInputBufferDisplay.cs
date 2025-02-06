using System.Text;
using TMPro;
using UnityEngine;

namespace SuperSmashRhodes.UI {
public class DebugInputBufferDisplay : PerSideUIElement<DebugInputBufferDisplay> {
    public TMP_Text text;
    
    private void Start() { }

    private void Update() {
        if (!player) return;
        var buffer = player.inputProvider.inputBuffer;
        
        StringBuilder sb = new();
        foreach (var chord in buffer.buffer) {
            if (chord.inputs.Length > 0) sb.AppendLine(string.Join(" ", chord.inputs));
            else sb.AppendLine("-");
        }

        text.text = sb.ToString();
    }
}
}
