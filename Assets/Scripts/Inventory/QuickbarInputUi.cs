using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(InventoryUi))]
public class QuickbarInputUi : MonoBehaviour
{
   [SerializeField] private InventoryUi quickbarUi;
    private CombatControls controls;
    private int selectedIndex;

    private void Awake() {
        controls = new CombatControls();
        if(quickbarUi == null) quickbarUi = GetComponent<InventoryUi>();
    }

    private void OnEnable()
    {
        TryInitHighlight();
        controls.Ui.QuickBarNavigate.performed += OnNavigate;
        controls.Ui.Enable();
    }
    private void OnDisable()
    {
        controls.Ui.QuickBarNavigate.performed -= OnNavigate;
        controls.Ui.Disable();
        ClearAllHighlights();
    }

    private void Start() {
        TryInitHighlight();
    }

    private void TryInitHighlight()
    {
        if(quickbarUi == null || quickbarUi.slotsUI == null || quickbarUi.slotsUI.Count == 0) return;
        selectedIndex = Mathf.Clamp(selectedIndex, 0, quickbarUi.slotsUI.Count -1);
        ClearAllHighlights();
        quickbarUi.slotsUI[selectedIndex].SetHighlight(true);
    }

    private void OnNavigate(InputAction.CallbackContext context)
    {
        var input = context.ReadValue<Vector2>();
        var newIndex = selectedIndex;

        if(input.x > 0.5f && selectedIndex < quickbarUi.slotsUI.Count - 1) newIndex = selectedIndex + 1;
        else if (input.x < -0.5f && selectedIndex > 0) newIndex = selectedIndex - 1;

        if(newIndex != selectedIndex)
        {
            quickbarUi.slotsUI[selectedIndex].SetHighlight(false);
            selectedIndex = newIndex;
            quickbarUi.slotsUI[selectedIndex].SetHighlight(true);
        }
    }

    private void ClearAllHighlights()
    {
        for (int i = 0; i < quickbarUi.slotsUI.Count; i++)
        {
            quickbarUi.slotsUI[i].SetHighlight(false);
        }
    }
}
