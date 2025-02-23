using UnityEngine;
using UnityEngine.UI;

namespace SuperSmashRhodes.UI.Util {

[RequireComponent(typeof(Graphic))]
public class InvertColor : MonoBehaviour
{
    private Material invertMaterial;
    private Graphic graphic;
    
    private void Awake()
    {
        graphic = GetComponent<Graphic>();
        invertMaterial = new Material(Shader.Find("SSR/InvertColor"));
        graphic.material = invertMaterial;
    }
}

}
