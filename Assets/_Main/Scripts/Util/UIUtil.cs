using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SuperSmashRhodes.UI {
public static class UIUtil {

    public static GameObject SetText(this GameObject go, string inText) {
        go.GetComponent<TextMeshProUGUI>().text = inText;

        return go;
    }

    public static string GetText(this GameObject go) {
        return go.GetComponent<TextMeshProUGUI>().text;
    }


    public static string GetInputText(this GameObject go) {
        return go.GetComponent<TMP_InputField>().text;
    }

    public static void SetInputText(this GameObject go, string text) {
        go.GetComponent<TMP_InputField>().text = text;
    }

    public static void SetInputPlaceholder(this TMP_InputField go, string text) {
        ((TextMeshProUGUI)go.placeholder).text = text;
    }

    public static void SetOnClickListener(this Button go, UnityAction cb) {
        go.onClick.AddListener(cb);
    }

    public static void SetText(this Button go, string text) {
        go.transform.Find("Text (TMP)").gameObject.SetText(text);
    }

    public static void SetColor(this GameObject go, Color color) {
        go.GetComponent<Image>().color = color;
    }

    public static void SetFillAmount(this GameObject go, float amount) {
        go.GetComponent<Image>().fillAmount = amount;
    }

    public static void RebuildLayout(this Transform transform) {
        if (transform is not RectTransform) return;
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
    }

    public static void ScrollToTarget(this ScrollRect scrollRect, GameObject targetObject) {
        // Ensure the target object is part of the ScrollRect content
        RectTransform content = scrollRect.content;
        RectTransform targetRectTransform = targetObject.GetComponent<RectTransform>();

        // Calculate the normalized vertical position (1 = top, 0 = bottom)
        float contentHeight = content.rect.height;
        float targetPosition = targetRectTransform.anchoredPosition.y;
        float normalizedPosition = 1 - (targetPosition / contentHeight);

        // Clamp the position between 0 and 1 to avoid out-of-bounds scrolling
        normalizedPosition = Mathf.Clamp01(normalizedPosition);

        // Set the scroll position (for vertical scrolling, adjust for horizontal similarly)
        scrollRect.verticalNormalizedPosition = normalizedPosition;
    }
    

    public static Sprite BytesToSprite(byte[] bytes, Vector2 imageSize) {
        int width = (int)imageSize.x;
        int height = (int)imageSize.y;
        var tex = new Texture2D(width, height, TextureFormat.RGB24, false);


        // Load the image file into the texture
        if (!tex.LoadImage(bytes)) {
            return null;
        }
        tex.LoadImage(bytes);

        int size = Mathf.Min(tex.width, tex.height); // Find the smallest dimension
        int xOffset = (tex.width - size) / 2; // Calculate horizontal offset
        int yOffset = (tex.height - size) / 2; // Calculate vertical offset
        var squareRect = new Rect(xOffset, yOffset, size, size); // Create a square rect

        var sprite = Sprite.Create(tex, squareRect, new Vector2(width / 2f, height / 2f));
        return sprite;
    }

    public static byte[] SpriteToBytes(Sprite sprite) {
        return ResizeBytes(sprite.texture.EncodeToPNG(), (int)sprite.rect.width, (int)sprite.rect.height);
    }

    private static byte[] ResizeBytes(byte[] bytes, int width, int height) {
        var texture = new Texture2D(width, height);
        texture.LoadImage(bytes);
        return texture.EncodeToPNG();
    }

    public static bool IsElementClickable(RectTransform rectTransform) {

        // Create a pointer event data with the position of the button
        var pointerData = new PointerEventData(EventSystem.current) {
            position = rectTransform.TransformPoint(rectTransform.rect.center)
        };

        // Raycast using the GraphicRaycaster attached to the Canvas
        var raycastResults = new List<RaycastResult>();
        var raycaster = rectTransform.GetComponentInParent<Canvas>().GetComponent<GraphicRaycaster>();
        raycaster.Raycast(pointerData, raycastResults);

        // Check if the first result is the button's GameObject
        return raycastResults.Count > 0 && raycastResults[0].gameObject == rectTransform.gameObject;
    }
}
}
