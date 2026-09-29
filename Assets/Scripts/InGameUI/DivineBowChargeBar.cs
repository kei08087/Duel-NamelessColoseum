using UnityEngine;
using UnityEngine.UI;

// Screen-space UI that tracks the charging Archer independently of the ground effect.
public class DivineBowChargeBar : MonoBehaviour
{
    private Transform caster;
    private RectTransform fill;
    private float startedAt;
    private float duration;
    private Canvas canvas;

    public static DivineBowChargeBar Show(Transform caster, float duration)
    {
        if (caster == null)
            return null;

        GameObject root = new GameObject("Divine Bow Charge Bar", typeof(RectTransform), typeof(Canvas),
            typeof(CanvasScaler), typeof(DivineBowChargeBar));
        root.transform.SetParent(caster, false);
        DivineBowChargeBar bar = root.GetComponent<DivineBowChargeBar>();
        bar.caster = caster;
        bar.duration = Mathf.Max(0.001f, duration);
        bar.startedAt = Time.time;
        bar.canvas = root.GetComponent<Canvas>();
        bar.canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        bar.canvas.overrideSorting = true;
        bar.canvas.sortingOrder = 60;

        CanvasScaler scaler = root.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        RectTransform track = CreateImage(root.transform, "Track", new Color(0.05f, 0.07f, 0.1f, 0.85f));
        track.sizeDelta = new Vector2(120f, 14f);
        bar.fill = CreateImage(track, "Remaining Charge", new Color(0.3f, 0.9f, 1f, 0.95f));
        bar.fill.anchorMin = new Vector2(0f, 0.5f);
        bar.fill.anchorMax = bar.fill.anchorMin;
        bar.fill.pivot = new Vector2(0f, 0.5f);
        bar.fill.anchoredPosition = new Vector2(2f, 0f);
        bar.fill.sizeDelta = new Vector2(116f, 10f);
        return bar;
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

    private void LateUpdate()
    {
        Camera view = Camera.main;
        if (caster == null || view == null || fill == null)
        {
            Hide();
            return;
        }

        Vector3 screen = view.WorldToScreenPoint(caster.position);
        canvas.enabled = screen.z > 0f;
        if (!canvas.enabled)
            return;

        // The camera looks down the world Z axis; a screen offset stays below
        // the character even while its attack direction changes.
        RectTransform track = (RectTransform)fill.parent;
        track.position = new Vector3(screen.x, screen.y - 55f * canvas.scaleFactor, 0f);
        float remaining = Mathf.Clamp01(1f - (Time.time - startedAt) / duration);
        fill.sizeDelta = new Vector2(116f * remaining, 10f);
    }

    public void Hide()
    {
        if (gameObject != null)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
