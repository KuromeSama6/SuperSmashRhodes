using System.Collections.Generic;
using UnityEngine;

namespace SuperSmashRhodes.FX {
[CreateAssetMenu(fileName = "New Flipbook Data", menuName = "SSR/FX/Flipbook Data", order = 0)]
public class FlipbookData : ScriptableObject {
    public int frames;
    public float frameRate = 60f;
    public List<Sprite> sprites = new List<Sprite>();
    
}
}
