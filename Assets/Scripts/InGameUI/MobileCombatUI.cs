using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MobileCombatUI : MonoBehaviour
{
    private static readonly string[] SkillSlots = { "RClick", "Q", "E", "LShift", "Space", "LCtrl" };
    private GameObject root;

    private void Update()
    {
        if (root != null && root.activeSelf && GameManager.Instance != null && GameManager.Instance.gameEnd)
            root.SetActive(false);
    }

    public void Initialize(CombatInputSource input, SkillsetBase skillset)
    {
        root = new GameObject("Mobile Combat Controls", typeof(RectTransform), typeof(Canvas),
            typeof(CanvasScaler), typeof(GraphicRaycaster));
        root.transform.SetParent(transform, false);
        Canvas canvas = root.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        canvas.sortingOrder = 50;
        CanvasScaler scaler = root.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        RectTransform stick = CreateSurface(root.transform, "Move Joystick", new Vector2(190f, 185f),
            new Vector2(230f, 230f), false, new Color(0.15f, 0.2f, 0.25f, 0.55f));
        RectTransform handle = CreateSurface(stick, "Handle", Vector2.zero,
            new Vector2(85f, 85f), false, new Color(1f, 1f, 1f, 0.6f));
        handle.anchorMin = new Vector2(0.5f, 0.5f);
        handle.anchorMax = handle.anchorMin;
        handle.anchoredPosition = Vector2.zero;
        handle.GetComponent<Image>().raycastTarget = false;
        MobileJoystick joystick = stick.gameObject.AddComponent<MobileJoystick>();
        joystick.Initialize(input, stick, handle);

        CreateButton(root.transform, input, "LClick", "ATK", new Vector2(160f, 145f),
            new Vector2(145f, 145f), new Color(0.75f, 0.27f, 0.2f, 0.8f));

        for (int i = 0; i < SkillSlots.Length; i++)
        {
            string slot = SkillSlots[i];
            Skill skill = skillset.getSkill(slot);
            string label = skill != null ? skill.skillID : "LOCKED";
            Vector2 position = new Vector2(100f + 105f * (i % 3), 335f + 105f * (i / 3));
            RectTransform button = CreateButton(root.transform, input, slot, label, position,
                new Vector2(92f, 92f), skill == null
                    ? new Color(0.25f, 0.25f, 0.25f, 0.45f)
                    : new Color(0.2f, 0.38f, 0.6f, 0.8f));
            if (skill == null)
                button.GetComponent<Image>().raycastTarget = false;
        }
    }

    private static RectTransform CreateButton(Transform parent, CombatInputSource input, string slot,
        string label, Vector2 position, Vector2 size, Color color)
    {
        RectTransform rect = CreateSurface(parent, slot, position, size, true, color);
        MobileCombatButton button = rect.gameObject.AddComponent<MobileCombatButton>();
        button.Initialize(input, slot);

        GameObject textObject = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(rect, false);
        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        text.text = label;
        text.alignment = TextAlignmentOptions.Center;
        text.fontSize = slot == "LClick" ? 31f : 16f;
        text.color = Color.white;
        text.raycastTarget = false;
        return rect;
    }

    private static RectTransform CreateSurface(Transform parent, string name, Vector2 position,
        Vector2 size, bool right, Color color)
    {
        GameObject surface = new GameObject(name, typeof(RectTransform), typeof(Image));
        surface.transform.SetParent(parent, false);
        RectTransform rect = surface.GetComponent<RectTransform>();
        rect.anchorMin = right ? new Vector2(1f, 0f) : new Vector2(0f, 0f);
        rect.anchorMax = rect.anchorMin;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = right ? new Vector2(-position.x, position.y) : position;
        Image image = surface.GetComponent<Image>();
        image.color = color;
        return rect;
    }
}

public class MobileJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private CombatInputSource input;
    private RectTransform area;
    private RectTransform handle;

    public void Initialize(CombatInputSource source, RectTransform areaRect, RectTransform handleRect)
    {
        input = source;
        area = areaRect;
        handle = handleRect;
    }

    public void OnPointerDown(PointerEventData eventData) => OnDrag(eventData);

    public void OnDrag(PointerEventData eventData)
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                area, eventData.position, eventData.pressEventCamera, out Vector2 local))
            return;

        float radius = area.rect.width * 0.4f;
        Vector2 clamped = Vector2.ClampMagnitude(local, radius);
        handle.anchoredPosition = clamped;
        input.SetJoystick(clamped / radius);
    }

    public void OnPointerUp(PointerEventData eventData) => ResetStick();

    private void OnDisable() => ResetStick();

    private void ResetStick()
    {
        if (handle != null) handle.anchoredPosition = Vector2.zero;
        if (input != null) input.ReleaseJoystick();
    }
}

public class MobileCombatButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private CombatInputSource input;
    private string slot;
    private Vector2 pressPosition;

    public void Initialize(CombatInputSource source, string skillSlot)
    {
        input = source;
        slot = skillSlot;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (slot == "LClick")
            pressPosition = eventData.position;
        else
            input.Press(slot, Vector3.zero);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (slot != "LClick") return;

        Vector2 drag = eventData.position - pressPosition;
        float threshold = 24f * GetComponentInParent<Canvas>().scaleFactor;
        Vector3 direction = drag.magnitude >= threshold
            ? new Vector3(drag.x, 0f, drag.y).normalized
            : Vector3.zero;
        input.Press(slot, direction);
    }
}
