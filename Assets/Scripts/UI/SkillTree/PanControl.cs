using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PanControl : MonoBehaviour
{
    [SerializeField]
    private InputActionAsset inputActionAsset;
    private InputAction panAction;
    private InputAction leftClickAction;

    private Vector2 currentPan;
    private bool isPanning;

    [SerializeField]
    private ScrollRect scrollRect;

    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();

        panAction = inputActionAsset.FindActionMap("UI").FindAction("Pan");
        panAction.performed += OnPan;
        panAction.canceled += OnPanEnd;

        leftClickAction = inputActionAsset.FindActionMap("UI").FindAction("LeftClick");
        leftClickAction.performed += OnLeftClickPerformed;
        leftClickAction.canceled += OnLeftClickCanceled;
    }

    private void OnEnable()
    {
        panAction.Enable();
        leftClickAction.Enable();
    }

    private void OnDisable()
    {
        panAction.Disable();
        leftClickAction.Disable();
    }

    private void Update()
    {
        if (isPanning)
        {
            Vector2 newPan = currentPan * new Vector2(0.0001f, 0.0005f);
            scrollRect.normalizedPosition -= newPan;
        }
    }

    private void OnLeftClickPerformed(InputAction.CallbackContext context)
    {
        isPanning = true;
    }

    private void OnLeftClickCanceled(InputAction.CallbackContext context)
    {
        isPanning = false;
    }

    private void OnPan(InputAction.CallbackContext context)
    {
        if (isPanning)
        {
            currentPan = context.ReadValue<Vector2>();
        }
    }

    private void OnPanEnd(InputAction.CallbackContext context)
    {
        currentPan = Vector2.zero;
    }
}
