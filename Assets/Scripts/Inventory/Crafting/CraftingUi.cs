using System.Collections.Generic;
using UnityEngine;

public class CraftingUi : MonoBehaviour
{
    [SerializeField] private Crafter crafter;
    [SerializeField] GameObject buttonUiPrefab;
    [SerializeField] List<GameObject> buttons = new();

    void OnEnable()
    {
        PopulateButtonsLIst();
    }

    public void PopulateButtonsLIst()
    {
        ClearAllButtons();
        foreach (Recipe recipe in crafter.recipeDatabase.GetCraftableRecipes(crafter.inventoryManager, crafter.currentStationType))
        {
            var button = Instantiate(buttonUiPrefab, transform);
            button.GetComponent<CraftingButtonUi>()?.Configure(recipe, () => crafter.TryCraft(recipe));
            buttons.Add(button);
        }
    }

    private void ClearAllButtons()
    {
        foreach (GameObject button in buttons)
        {
            Destroy(button);
        }
        buttons.Clear();
    }
}
