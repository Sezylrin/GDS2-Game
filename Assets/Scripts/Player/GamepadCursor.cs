using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class GamepadCursor : MonoBehaviour
{
    [SerializeField]
    private EventSystem eventSystem;
    [SerializeField]
    private Image cursorImage;
    [SerializeField]
    private InputActionAsset inputActionAsset;
    [SerializeField]
    private float cursorSpeed = 1500.0f;
    [SerializeField]
    private float joystickScrollSpeed = 0.001f;
    [SerializeField]
    private Canvas SkillTreeCanvas;
    [SerializeField]
    private ScrollRect scrollRect;

    private InputAction moveCursorAction;
    private InputAction clickAction;
    private InputAction scrollAction;

    private Vector2 currentPos;
    private Button currentlyHoveredButton = null;

    void Start()
    {
        var uiMap = inputActionAsset.FindActionMap("UI");
        moveCursorAction = uiMap.FindAction("MoveCursor");
        clickAction = uiMap.FindAction("Click");
        scrollAction = uiMap.FindAction("ScrollWheel");

        scrollAction.Enable();
        moveCursorAction.Enable();
        clickAction.Enable();

        clickAction.performed += OnClick;

        currentPos = Vector2.zero;
        cursorImage.rectTransform.anchoredPosition = currentPos;
    }

    void Update()
    {
        Vector2 scrollVector = scrollAction.ReadValue<Vector2>() * joystickScrollSpeed;
        Vector2 normalizedPos = scrollRect.normalizedPosition;
        normalizedPos.y = Mathf.Clamp01(normalizedPos.y + scrollVector.y);
        normalizedPos.x = Mathf.Clamp01(normalizedPos.x - scrollVector.x);
        scrollRect.normalizedPosition = normalizedPos;

        Vector2 input = moveCursorAction.ReadValue<Vector2>();
        currentPos += input * cursorSpeed * Time.unscaledDeltaTime;

        RectTransform canvasRect = SkillTreeCanvas.GetComponent<RectTransform>();
        currentPos.x = Mathf.Clamp(currentPos.x, 0, canvasRect.sizeDelta.x);
        currentPos.y = Mathf.Clamp(currentPos.y, 0, canvasRect.sizeDelta.y);

        cursorImage.rectTransform.anchoredPosition = currentPos;

        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = cursorImage.rectTransform.position;

        List<RaycastResult> results = new List<RaycastResult>();
        eventSystem.RaycastAll(pointerData, results);

        Button buttonUnderCursor = null;

        if (results.Count > 0)
        {
            foreach (var result in results)
            {
                Button button = result.gameObject.GetComponent<Button>();
                if (button != null)
                {
                    buttonUnderCursor = button;
                    break;
                }
            }
        }

        if (buttonUnderCursor != currentlyHoveredButton)
        {
            if (currentlyHoveredButton)
            {
                ExecuteEvents.Execute<IPointerExitHandler>(currentlyHoveredButton.gameObject, pointerData, ExecuteEvents.pointerExitHandler);
            }

            currentlyHoveredButton = buttonUnderCursor;

            if (currentlyHoveredButton)
            {
                ExecuteEvents.Execute<IPointerEnterHandler>(currentlyHoveredButton.gameObject, pointerData, ExecuteEvents.pointerEnterHandler);
            }
        }
    }

    private void OnClick(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && currentlyHoveredButton)
        {
            PointerEventData pointerData = new PointerEventData(eventSystem);
            pointerData.position = cursorImage.rectTransform.position;
            ExecuteEvents.Execute<IPointerClickHandler>(currentlyHoveredButton.gameObject, pointerData, ExecuteEvents.pointerClickHandler);
        }
    }

    private void OnDisable()
    {
        moveCursorAction.Disable();
        clickAction.performed -= OnClick;
        clickAction.Disable();
        scrollAction.Disable();
    }
}
