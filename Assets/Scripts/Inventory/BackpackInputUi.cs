using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(InventoryUi))]
public class BackpackInputUi : MonoBehaviour, IInventorySelectionUi
{
    [SerializeField] private InventoryUi inventoryUi;
    [SerializeField] private int columns = 6;

    private CombatControls controls;
    private int selectedIndex = 0;

    public InventorySlotUi CurrentSlotUi
    {
        get
        {
            if (inventoryUi?.slotsUI == null) return null;
            if (selectedIndex < 0 || selectedIndex >= inventoryUi.slotsUI.Count) return null;
            return inventoryUi.slotsUI[selectedIndex];
        }
    }

    public IReadOnlyList<InventorySlotUi> Slots => throw new System.NotImplementedException();

    private void Awake()
    {
        controls = new CombatControls();
        if(inventoryUi == null) inventoryUi = GetComponent<InventoryUi>();
    }

    private void OnEnable()
    {
        //test
        selectedIndex = 0;
        inventoryUi.slotsUI[selectedIndex].SetHighlight(true);

        controls.Ui.Navigate.performed += OnNavigate;
        controls.Ui.Enable();
    }

    private void OnDisable()
    {
        controls.Ui.Navigate.performed -= OnNavigate;
        controls.Ui.Disable();
        ClearAllHighlights();
    }

    private void OnDestroy() => controls?.Dispose();

    private void OnNavigate(InputAction.CallbackContext context)
    {
        var input = context.ReadValue<Vector2>();
        var col = selectedIndex % columns;
        var row = selectedIndex / columns;
        var newIndex = selectedIndex;

        if(input.x > 0.5f && col < columns -1) newIndex = selectedIndex + 1;
        else if (input.x < -0.5f && col > 0) newIndex = selectedIndex - 1;
        else if (input.y > 0.5f &&  selectedIndex - columns >= 0) newIndex = selectedIndex - columns;
        else if (input.y < -0.5f && selectedIndex + columns < inventoryUi.slotsUI.Count) newIndex = selectedIndex + columns;

        if(newIndex != selectedIndex)
        {
            inventoryUi.slotsUI[selectedIndex].SetHighlight(false);
            selectedIndex = newIndex;
            inventoryUi.slotsUI[selectedIndex].SetHighlight(true);
        }
              
    }

    private void ClearAllHighlights()
    {
        for (int i = 0; i < inventoryUi.slotsUI.Count; i++)
        {
            inventoryUi.slotsUI[i].SetHighlight(false);
        }
    }

    public void Redraw()
    {
        inventoryUi?.Redraw();
    }
}
