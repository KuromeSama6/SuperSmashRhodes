using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace SuperSmashRhodes.Util {
public static class GameUtil {
    public static float InverseLerp(float a, float b, float value) {
        if (a == b) {
            return 0f;
        }
        return (value - a) / (b - a);
    }

    public static T[] GetChildrenOfObject<T>(GameObject parent) {
        var ret = new List<T>();
        for (int i = 0; i < parent.transform.childCount; i++) {
            var child = parent.transform.GetChild(i).gameObject;

            T comp;
            if (child.TryGetComponent(out comp)) {
                ret.Add(comp);
            }
        }

        return ret.ToArray();
    }

    public static Color HexToColor(this string hex) {
        byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
        byte a = byte.Parse(hex.Length == 8 ? hex.Substring(6, 2) : "ff", NumberStyles.HexNumber);

        return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }

    public static Color Invert(this Color color) {
        float inverseR = 1.0f - color.r;
        float inverseG = 1.0f - color.g;
        float inverseB = 1.0f - color.b;

        return new Color(inverseR, inverseG, inverseB, color.a);
    }

    public static string ToStringRounded(this float num) {
        return Mathf.RoundToInt(num).ToString();
    }

    public static bool WithinRange(Vector2 range, float value) {
        return value >= range.x && value < range.y;
    }

    public static LTDescr AnimatedSetActive(this GameObject obj, bool active, float delay = 0.1f, Action callback = null) {
        // new canvasgroup
        if (obj.activeInHierarchy == active) return null;
        if (obj.GetComponent<CanvasGroup>() == null) obj.AddComponent<CanvasGroup>();

        var canvas = obj.GetComponent<CanvasGroup>();
        if (canvas.alpha < 1 && canvas.alpha > 0) return null;
        canvas.alpha = active ? 0f : 1f;
        if (active) obj.SetActive(active);
        var anim = LeanTween.alphaCanvas(canvas, active ? 1f : 0f, delay).setOnComplete(() => {
            obj.SetActive(active);
            callback?.Invoke();
        });

        return anim;
    }

    public static Color LerpColor(Color a, Color b, float t) {
        return new Color(
            Mathf.Lerp(a.r, b.r, t),
            Mathf.Lerp(a.g, b.g, t),
            Mathf.Lerp(a.b, b.b, t),
            Mathf.Lerp(a.a, b.a, t)
        );
    }

    public static Task TaskDelay(int time) {
        return Task.Delay(time);
    }
    public static Color ApplyAlpha(this Color color, float alpha) {
        return new Color(color.r, color.g, color.b, alpha);
    }

    public static GameObject[] GetChildren(this GameObject parent) {
        var ret = new List<GameObject>();
        for (int i = 0; i < parent.transform.childCount; i++) {
            var child = parent.transform.GetChild(i).gameObject;
            ret.Add(child);
        }

        return ret.ToArray();
    }

    public static void ClearChildren(this GameObject parent) {
        foreach (var child in parent.GetChildren()) Object.Destroy(child);
    }

    public static GameObject FindChild(this GameObject parent, string name) {
        return parent.GetChildren().ToList().Find(c => c.name == name);
    }

    public static float Distance(this Vector3 v1, Vector3 v2) {
        return Vector3.Distance(v1, v2);
    }

    public static GameObject GetChild(this GameObject parent, int index) {
        return GetChildren(parent)[index];
    }

    public static GameObject[] GetFullObjectRecursive(GameObject parent) {
        var ret = new List<GameObject> {
            parent
        };

        for (int i = 0; i < parent.transform.childCount; i++) {
            var child = parent.transform.GetChild(i).gameObject;
            ret.Add(child);

            if (child.transform.childCount > 0) {
                ret = ret.Concat(new List<GameObject>(GetFullObjectRecursive(child))).ToList();
            }

        }

        return ret.ToArray();
    }

    public static T RandomFromArray<T>(T[] array) {
        return array[new Random().Next(0, array.Length)];
    }

    public static float DistPointFromLine(Vector2 vector2Point, Vector2 linePoint1, Vector2 linePoint2) {
        // Define the Vector2 point and the two points that define the line

        // Find the slope of the line using the formula m = (y2 - y1) / (x2 - x1)
        float slope = (linePoint2.y - linePoint1.y) / (linePoint2.x - linePoint1.x);

        // Find the y-intercept of the line using the formula b = y - mx
        float yIntercept = linePoint1.y - slope * linePoint1.x;

        // Substitute the coordinates of the vector2 point into the equation of the line to find the y-coordinate
        float y = slope * vector2Point.x + yIntercept;

        // Calculate the difference between the vector2 point and the line
        float difference = vector2Point.y - y;

        return difference;
    }

    public static float Max0(this float val) {
        return Mathf.Max(0, val);
    }

    public static float Min0(this float val) {
        return Mathf.Min(0, val);
    }

    public static float Clamp0(this float val) {
        return val > 0 ? Mathf.Max(0, val) : Mathf.Min(0, val);
    }

    public static Vector2 GetPointFromOrigin(Vector2 origin, float angle, float distance) {
        // Convert the angle from degrees to radians
        float radians = angle * Mathf.Deg2Rad;

        // Use trigonometry to calculate the new point's x and y coordinates
        float x = origin.x + distance * Mathf.Cos(radians);
        float y = origin.y + distance * Mathf.Sin(radians);

        // Return the new point as a Vector2
        return new Vector2(x, y);
    }

    public static T FindComponentInParent<T>(this GameObject go, Predicate<T> pred = null) where T : MonoBehaviour {
        var comp = go.GetComponent<T>();
        if (comp != null && (pred == null || pred.Invoke(comp))) return comp;

        // look in the parent object
        if (go.transform.parent != null) {
            return go.transform.parent.gameObject.FindComponentInParent(pred);
        }
        return null;
    }

    public static Keyframe[] RandomCurve(Vector2 min, Vector2 max, int count) {
        var frames = new List<Keyframe>();

        float[] xRand = GenerateRandomFloats(min.x, max.x, count);
        float[] yRand = GenerateRandomFloats(min.y, max.y, count);

        for (int i = 0; i < count - 1; i++) {
            //frames.Add(new(xRand[i], yRand[i]));
            frames.Add(new Keyframe(((float)count + 1) / count * (i + 1f), yRand[i]));
        }

        return frames.ToArray();
    }

    public static float[] GenerateRandomFloats(float min, float max, int count) {
        if (count <= 0) {
            throw new ArgumentException("Count must be greater than zero.");
        }

        if (min >= max) {
            throw new ArgumentException("Min must be less than max.");
        }

        var ret = new List<float>();
        for (int i = 0; i < count; i++) ret.Add(UnityEngine.Random.Range(min, max));

        return ret.ToArray();
    }

    public static void RecursiveCopyTransform(Transform from, Transform to) {
        var toChildren = to.gameObject.GetChildren().ToList();
        foreach (var child in from.gameObject.GetChildren()) {
            var toChild = toChildren.Find(c => c.gameObject.name == child.gameObject.name);
            if (toChild == null) continue;

            toChild.transform.localPosition = child.transform.localPosition;
            toChild.transform.localRotation = child.transform.localRotation;
            toChild.transform.localScale = child.transform.localScale;

            if (child.transform.childCount > 0) RecursiveCopyTransform(child.transform, toChild.transform);

        }
    }

    public static bool IsPointInBox(Vector2 corner1, Vector2 corner2, Vector2 point) {
        // Find the minimum and maximum X and Y values between the two corners
        float minX = Mathf.Min(corner1.x, corner2.x);
        float maxX = Mathf.Max(corner1.x, corner2.x);
        float minY = Mathf.Min(corner1.y, corner2.y);
        float maxY = Mathf.Max(corner1.y, corner2.y);

        // Check if the point's X and Y are within the bounds
        return point.x >= minX && point.x <= maxX && point.y >= minY && point.y <= maxY;
    }

    public static void RecursiveCopyTransformCoroutine(MonoBehaviour owner, Transform from, Transform to, Action onDone = null) {
        owner.StartCoroutine(Run());

        IEnumerator Run() {
            var toChildren = to.gameObject.GetChildren().ToList();
            foreach (var child in from.gameObject.GetChildren()) {
                var toChild = toChildren.Find(c => c.gameObject.name == child.gameObject.name);
                if (toChild == null) continue;

                toChild.transform.localPosition = child.transform.localPosition;
                toChild.transform.localRotation = child.transform.localRotation;
                toChild.transform.localScale = child.transform.localScale;

                if (child.transform.childCount > 0) RecursiveCopyTransformCoroutine(owner, child.transform, toChild.transform);
                yield return 0;
            }

            onDone?.Invoke();
        }
    }

    public static IEnumerator RunAsyncCoroutine<T>(Func<T> call, Action<T> callback = null, Action<Exception> onError = null) {
        var task = Task.Run(call);
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.IsCompletedSuccessfully) {
            callback?.Invoke(task.Result);
        } else {
            if (onError != null) onError.Invoke(task.Exception);
            else Debug.LogException(task.Exception);
        }
    }

    public static void RunAsyncCoroutine<T>(this MonoBehaviour owner, Func<T> call, Action<T> callback = null, Action<Exception> onError = null) {
        owner.StartCoroutine(RunAsyncCoroutine(call, callback, onError));
    }
    
    public static T[] EnumValues<T>() where T : Enum {
        return (T[])Enum.GetValues(typeof(T));
    }

    public static T RandomChoice<T>(this List<T> li) {
        if (li.Count == 0) return default;
        return li[UnityEngine.Random.Range(0, li.Count - 1)];
    }

    public static U GetOrDefault<T, U>(this IDictionary<T, U> dict, T key, U fallback = default) {
        if (!dict.ContainsKey(key)) return fallback;
        return dict[key];
    }

    public static U FindGetOrDefault<T, U>(this IDictionary<T, U> dict, Predicate<T> pred, U fallback = default) {
        var item = dict.Keys.ToList().Find(pred);
        if (item == null) return fallback;

        return dict[item];
    }

    public static void Reset(this Transform transform, bool keepScale = true) {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        if (!keepScale) transform.localScale = Vector3.one;
    }

    public static Coroutine CallLaterCoroutine(this MonoBehaviour beh, float delay, Action action) {
        IEnumerator Run() {
            yield return new WaitForSeconds(delay);
            action.Invoke();
        }

        return beh.StartCoroutine(Run());
    }
    
    public static Coroutine CallNextFrameCoroutine(this MonoBehaviour beh, Action action) {
        IEnumerator Run() {
            yield return new WaitForEndOfFrame();
            yield return null;
            action.Invoke();
        }

        return beh.StartCoroutine(Run());
    }

    public static string FormatTime(int seconds) {
        var timeSpan = TimeSpan.FromSeconds(seconds);
        return $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
    }

    public static long ToTimestamp(this DateTime dateTime) {
        // Convert to UTC to ensure accurate calculation
        var dateTimeUtc = dateTime.ToUniversalTime();

        // Calculate the number of milliseconds since Unix epoch
        return (long)(dateTimeUtc - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
    }

    public static DateTime ToDateTime(this long epochMilliseconds) {

        // Create a DateTime object from epoch milliseconds
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(epochMilliseconds);

        // Convert it to the local time zone if needed
        return dateTime.ToLocalTime();
    }

    public static void RemoveIf<TKey, TValue>(this Dictionary<TKey, TValue> dict, Predicate<KeyValuePair<TKey, TValue>> predicate) {
        var keysToRemove = new List<TKey>();

        foreach (var kvp in dict) {
            if (predicate(kvp)) {
                keysToRemove.Add(kvp.Key);
            }
        }

        foreach (var key in keysToRemove) {
            dict.Remove(key);
        }
    }

    public static void RunTaskCoroutine<T>(this MonoBehaviour owner, CoroutineWrapper<T> coroutine, Action<T> callback = null) {
        IEnumerator Coroutine() {
            yield return coroutine;
            callback?.Invoke(coroutine.result);
        }
        
        owner.StartCoroutine(Coroutine());
    }

    public static string ToPrettyName(string camelCaseName) {
        if (string.IsNullOrEmpty(camelCaseName))
            return camelCaseName;

        // Insert spaces before uppercase letters
        string prettyName = Regex.Replace(
            camelCaseName,
            "(\\B[A-Z])",
            " $1"
        );

        // Capitalize the first letter
        return char.ToUpper(prettyName[0]) + prettyName.Substring(1);
    }

    public static bool Contains(this Vector2 range, float point) {
        return range.x <= point && range.y > point;
    }
}

[Serializable]
public struct LerpedFloat {
    public float value {
        get {
            return _value;
        }
        set {
            target = value;
        }
    }

    [HideInInspector] public float _value, target;

    public bool isIncreasing {
        get {
            return _value < target;
        }
    }
    public bool isDecreasing {
        get {
            return _value > target;
        }
    }

    public void Update(float multiplier = 1f) {
        _value = Mathf.Lerp(_value, target, Time.deltaTime * multiplier);
    }

    public void HardSet(float amount) {
        _value = amount;
        target = amount;
    }

}

[Serializable]
public struct RandomCurveGenerator {
    [HideInInspector] public AnimationCurve curve;
    public int interval;
    public float min, max;

    [Header("Runtime")]
    public AnimationCurve curveDisplay;

    public float counter { get; private set; }
    public float value {
        get {
            return curve.Evaluate(counter);
        }
    }

    public RandomCurveGenerator(int interval, float min, float max) : this() {
        this.interval = interval;
        this.max = max;
        this.min = min;
        curve = GenerateCurve();
    }

    public void Update() {
        if (counter > interval + 1) Reset();

        curveDisplay = curve;
        counter += Time.deltaTime;
    }

    public void Reset() {
        curve = GenerateCurve();
        counter = 0;
    }

    public AnimationCurve GenerateCurve() {
        return NormalizedCurve(interval, min, max);
    }

    public static AnimationCurve NormalizedCurve(int interval, float min, float max) {
        var frames = GameUtil.RandomCurve(new Vector2(1f, min), new Vector2(interval - 1, max), interval).ToList();
        frames.Insert(0, new Keyframe(0, min.Max0()));
        frames.Add(new Keyframe(interval + 1, min.Max0()));
        return new AnimationCurve(frames.ToArray());
    }
}

public class CursorState {
    public readonly CursorLockMode mode;
    public readonly bool visible;

    public CursorState() {
        mode = Cursor.lockState;
        visible = Cursor.visible;
    }

    public void Apply() {
        Cursor.lockState = mode;
        Cursor.visible = visible;
    }

    public static void SetCursorUsable(bool usable) {
        Cursor.lockState = usable ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = usable;
    }
}

public static class RectTransformExtensions {
    public static Vector2 GetLeftCorner(this RectTransform rectTransform) {
        var worldCorners = new Vector3[4];
        rectTransform.GetWorldCorners(worldCorners);
        return new Vector2(worldCorners[0].x, worldCorners[0].y); // Bottom-left corner
    }

    public static Vector2 GetRightCorner(this RectTransform rectTransform) {
        var worldCorners = new Vector3[4];
        rectTransform.GetWorldCorners(worldCorners);
        return new Vector2(worldCorners[2].x, worldCorners[2].y); // Top-right corner
    }

    public static Vector2 GetLeftCornerCameraSpace(this RectTransform rectTransform, Camera uiCamera) {
        // Get the world corners of the RectTransform
        var worldCorners = new Vector3[4];
        rectTransform.GetWorldCorners(worldCorners);

        // Convert the bottom-left corner from world space to screen space
        var screenPosition = uiCamera.WorldToScreenPoint(worldCorners[0]);

        // Return the screen space position
        return new Vector2(screenPosition.x, screenPosition.y);
    }

    public static Vector2 GetRightCornerCameraSpace(this RectTransform rectTransform, Camera uiCamera) {
        // Get the world corners of the RectTransform
        var worldCorners = new Vector3[4];
        rectTransform.GetWorldCorners(worldCorners);

        // Convert the top-right corner from world space to screen space
        var screenPosition = uiCamera.WorldToScreenPoint(worldCorners[2]);

        // Return the screen space position
        return new Vector2(screenPosition.x, screenPosition.y);
    }
}

}
