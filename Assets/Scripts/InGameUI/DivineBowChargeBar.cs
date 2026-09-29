using UnityEngine;
using UnityEngine.UI;

// Screen-space UI that tracks the charging Archer independently of the ground effect.
public class DivineBowChargeBar : MonoBehaviour
{
    private Transform caster;
    private RectTransform fill;
    private RectTransform track;
    private float startedAt;
    private float duration;
    private Canvas canvas;
    public float RemainingFraction { get; private set; } = 1f;

    public static DivineBowChargeBar Show(Transform caster, float duration)
    {
        if (caster == null)
            return null;

        Transform existing = caster.Find("Divine Bow Charge Bar");
        DivineBowChargeBar bar = existing != null ? existing.GetComponent<DivineBowChargeBar>() : null;
        if (bar == null)
        {
            GameObject root = new GameObject("Divine Bow Charge Bar", typeof(RectTransform), typeof(Canvas),
                typeof(CanvasScaler), typeof(DivineBowChargeBar));
            root.transform.SetParent(caster, false);
            bar = root.GetComponent<DivineBowChargeBar>();
            bar.Initialize();
        }
        bar.caster = caster;
        bar.duration = Mathf.Max(0.001f, duration);
        bar.startedAt = Time.time;
        bar.gameObject.SetActive(true);
        bar.Refresh(Camera.main, 0f);
        return bar;
    }

    private void Initialize()
    {
        canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        canvas.sortingOrder = 60;

        CanvasScaler scaler = GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        track = CreateImage(transform, "Track", new Color(0.05f, 0.07f, 0.1f, 0.85f));
        track.sizeDelta = new Vector2(120f, 14f);
        fill = CreateImage(track, "Remaining Charge", new Color(0.3f, 0.9f, 1f, 0.95f));
        fill.anchorMin = new Vector2(0f, 0.5f);
        fill.anchorMax = fill.anchorMin;
        fill.pivot = new Vector2(0f, 0.5f);
        fill.anchoredPosition = new Vector2(2f, 0f);
        fill.sizeDelta = new Vector2(116f, 10f);
    }

    private static RectTransform CreateImage(Transform parent, string name, Color color)
    {
        GameObject surface = new GameObject(name, typeof(RectTransform), typeof(Image));
        surface.transform.SetParent(parent, false);
        Image image = surface.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        return surface.GetComponent<RectTransform>();
    }

    private void LateUpdate() => Refresh(Camera.main, Time.time - startedAt);

    // Elapsed seconds and camera are explicit so UI positioning can be checked
    // without relying on a private timer or a particular viewport size.
    public void Refresh(Camera view, float elapsedSeconds)
    {
        if (fill == null || track == null || canvas == null)
            return;
        RemainingFraction = Mathf.Clamp01(1f - elapsedSeconds / duration);
        fill.sizeDelta = new Vector2(116f * RemainingFraction, 10f);

        if (caster == null || view == null)
        {
            canvas.enabled = false;
            return;
        }

        // Project a world-space offset along the camera's down direction.
        // This stays below the fighter at different zooms and aspect ratios.
        Vector3 screen = view.WorldToScreenPoint(caster.position - view.transform.up * 0.85f);
        canvas.enabled = screen.z > 0f;
        if (!canvas.enabled)
            return;
        track.position = new Vector3(screen.x, screen.y, 0f);
    }

    public void Hide()
    {
        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }
}
