using System.Collections.Generic;
using System.Linq;
using UnityEngine;



/// <summary>
/// Small recipe database for crafting stations.
///
/// Responsibilities:
/// - Store available <see cref="Recipe"/> entries
/// - Provide lookups for recipes valid at a specific <see cref="StationType"/>
/// - Evaluate whether a recipe is craftable given an <see cref="InventoryContainer"/>
///
/// This service resides on a scene <see cref="MonoBehaviour"/> and keeps recipes
/// configurable via the Inspector. It does not perform the actual crafting (which
/// should be handled by a crafting controller or UI) — it only answers queries.
/// </summary>
public class RecipeDatabase : MonoBehaviour
{
    [SerializeField] private List<Recipe> recipes = new();

    /// <summary>
    /// Read-only access to the full recipe list configured on this database.
    /// </summary>
    public IReadOnlyList<Recipe> All => recipes;

    /// <summary>
    /// Returns recipes that are available for the given <paramref name="stationType"/>.
    /// A recipe with <see cref="StationType.Null"/> is considered station-agnostic and
    /// will be returned for all station queries.
    /// </summary>
    public IEnumerable<Recipe> ForStation(StationType stationType) => recipes.Where(r => r.stationType == StationType.Null || r.stationType == stationType);

    /// <summary>
    /// Returns <c>true</c> when the given <paramref name="inventory"/> contains the required
    /// ingredients and the <paramref name="recipe"/> is valid for the provided <paramref name="station"/>.
    /// </summary>
    /// <remarks>
    /// Rules applied:
    /// - If <paramref name="recipe"/> requires a specific station (not <see cref="StationType.Null"/>)
    ///   the check fails when the current <paramref name="station"/> doesn't match.
    /// - Each ingredient's required quantity is verified against <see cref="InventoryContainer.GetItemQuantity"/>.
    /// </remarks>
    public bool CanCraft(InventoryContainer inventory, Recipe recipe, StationType station)
    {
        // se nao estiver na estação, nao rola
        if (recipe.stationType != StationType.Null && recipe.stationType != station)
        {
            return false;
        }
        //check nos ingredientes
        foreach (var ingredient in recipe.ingredients)
        {
            if (inventory.GetItemQuantity(ingredient.item) < ingredient.quantity)
            {
                return false;
            }
        }
        return true;
    }
    /// <summary>
    /// Returns the subset of recipes available at <paramref name="station"/> that
    /// the provided <paramref name="inventory"/> currently has the ingredients to craft.
    /// </summary>
    public IEnumerable<Recipe> GetCraftableRecipes(InventoryContainer inventory, StationType station) =>
        ForStation(station).Where(r => CanCraft(inventory, r, station));
}
