using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple UI component that populates a list of crafting buttons for the active
/// station using a <see cref="Crafter"/> instance.
///
/// Responsibilities:
/// - Query the <see cref="RecipeDatabase"/> for craftable recipes at the
///   current station (via the <see cref="Crafter"/>)
/// - Instantiate and configure a button prefab for each craftable recipe
/// - Manage created UI instances and clear them when rebuilding
///
/// Notes:
/// - This component instantiates a prefab per recipe and calls
///   <see cref="CraftingButtonUi.Configure(Recipe,System.Action)"/> to bind behaviour.
/// - It relies on the Crafter to actually perform the craft when a button is pressed.
/// </summary>
public class CraftingUi : MonoBehaviour
{
    /// <summary>
    /// The crafter component used to test craftability and execute crafts.
    /// </summary>
    [SerializeField] private Crafter crafter;

    /// <summary>
    /// Prefab used for each recipe button. Expected to contain a CraftingButtonUi
    /// component which will be configured automatically.
    /// </summary>
    [SerializeField] GameObject buttonUiPrefab;

    /// <summary>
    /// Runtime list of currently active button instances created by this UI.
    /// </summary>
    [SerializeField] List<GameObject> buttons = new();

    /// <summary>
    /// Populate the UI whenever the object becomes enabled (scene/show). This
    /// automatically creates buttons for all craftable recipes at the current station.
    /// </summary>
    void OnEnable()
    {
        PopulateButtonsLIst();
    }

    /// <summary>
    /// Rebuilds the list of buttons based on the recipes the player can currently craft.
    /// Existing buttons are destroyed before new buttons are created.
    /// </summary>
    public void PopulateButtonsLIst()
    {
        // safety check — avoid null reference if setup isn't complete
        if (crafter == null || crafter.recipeDatabase == null || crafter.inventoryManager == null) return;

        ClearAllButtons();

        foreach (Recipe recipe in crafter.recipeDatabase.GetCraftableRecipes(crafter.inventoryManager, crafter.currentStationType))
        {
            var button = Instantiate(buttonUiPrefab, transform);
            // configure the button to call crafter.TryCraft when the user clicks it
            button.GetComponent<CraftingButtonUi>()?.Configure(recipe, () => crafter.TryCraft(recipe));
            buttons.Add(button);
        }
    }

    /// <summary>
    /// Remove and destroy all runtime-created button GameObjects, clearing the
    /// internal list.
    /// </summary>
    private void ClearAllButtons()
    {
        foreach (GameObject button in buttons)
        {
            Destroy(button);
        }
        buttons.Clear();
    }
}
