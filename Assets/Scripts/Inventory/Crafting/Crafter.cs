using UnityEngine;

/// <summary>
/// Small helper component that performs crafting using a <see cref="RecipeDatabase"/>
/// and the player's <see cref="InventoryManager"/>.
///
/// Responsibilities:
/// - Validate that a recipe can be crafted for the current station and active inventories
/// - Consume ingredients and add the resulting item back into the player's inventory
/// - Raise an event when the UI should be refreshed after a successful craft
///
/// Notes:
/// - The implementation delegates checking of available ingredients to
///   <see cref="RecipeDatabase.CanCraft(InventoryManager, Recipe, StationType)"/>.
/// - Consumption and addition use <see cref="InventoryManager.TryConsume"/> and
///   <see cref="InventoryManager.AddItem"/>; adjust if you need a different
///   priority for which container is consumed first.
/// </summary>
public class Crafter : MonoBehaviour
{
    [SerializeField] public RecipeDatabase recipeDatabase;
    /// <summary>
    /// The combined inventory manager used for ingredient checks and consumption.
    /// This aggregates the quick bar and backpack so the crafter doesn't need
    /// to know which container holds the items.
    /// </summary>
    [SerializeField] public InventoryManager inventoryManager; //backpack + quickbar
    [SerializeField] public StationType currentStationType = StationType.Null;

    [SerializeField] private GameEvent redrawUIEvent;

    /// <summary>
    /// Attempt to craft the provided <paramref name="recipe"/> at the current station.
    /// </summary>
    /// <remarks>
    /// Behavior:
    /// - Validates that both the recipe database and inventory manager are assigned
    ///   and that the recipe is craftable at the current station.
    /// - Consumes the recipe ingredients and attempts to add the crafted item. If
    ///   adding the result fails the ingredients are not rolled back in the current
    ///   implementation (you can change this to a transactional operation later).
    /// - Raises <see cref="redrawUIEvent"/> when crafting succeeds so UI can update.
    /// </remarks>
    /// <returns>True when the craft completed successfully and the result was added to inventory.</returns>
    public bool TryCraft(Recipe recipe)
    {
        // Basic null-safety checks
        if (inventoryManager == null || recipeDatabase == null || recipe == null) return false;

        // Use the InventoryManager-aware CanCraft to validate across both quickBar/backpack
        if (!recipeDatabase.CanCraft(inventoryManager, recipe, currentStationType))
        {
            Debug.Log("Cannot craft recipe: " + recipe.name);
            return false;
        }

        // Consume ingredients. We use the manager-level TryConsume to ensure combined
        // inventories are checked/consumed consistently. If TryConsume ever fails
        // it would indicate a race or inconsistency that CanCraft missed.
        foreach (var ingredient in recipe.ingredients)
        {
            if (!inventoryManager.TryConsume(ingredient.item, ingredient.quantity))
            {
                Debug.LogWarning($"Failed to consume ingredient {ingredient.item?.name} while crafting {recipe.name}.");
                return false;
            }
        }

        // Try to add crafted result back into player's inventories. If this fails
        // the ingredients are already consumed; consider making this transactional
        // if you need stronger guarantees.
        if (!inventoryManager.AddItem(recipe.resultItem, recipe.resultQuantity))
        {
            Debug.LogWarning($"Craft succeeded but could not add result {recipe.resultItem?.name} to inventory.");
            return false;
        }

        // notify listeners (UI etc.)
        redrawUIEvent?.Raise();
        return true;
    }
}
