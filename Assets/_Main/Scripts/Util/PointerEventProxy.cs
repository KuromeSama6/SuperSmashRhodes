using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace SuperSmashRhodes.Util {
public class PointerEventProxy : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler {
    public UnityEvent<PointerEventData> onClick { get; } = new();
    public UnityEvent<PointerEventData> onPointerDown { get; } = new();
    public UnityEvent<PointerEventData> onPointerUp { get; } = new();
    public UnityEvent<PointerEventData> onPointerEnter { get; } = new();
    public UnityEvent<PointerEventData> onPointerExit { get; } = new();
    public UnityEvent<PointerEventData> onPointerMove { get; } = new();

    public void OnPointerClick(PointerEventData eventData) {
        onClick.Invoke(eventData);
    }

    public void OnPointerDown(PointerEventData eventData) {
        onPointerDown.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData) {
        onPointerUp.Invoke(eventData);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        onPointerEnter.Invoke(eventData);
    }

    public void OnPointerExit(PointerEventData eventData) {
        onPointerExit.Invoke(eventData);
    }

    public void OnPointerMove(PointerEventData eventData) {
        onPointerMove.Invoke(eventData);
    }
}
}
