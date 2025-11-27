using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class RecipeDatabase : MonoBehaviour
{
    [SerializeField] private List<Recipe> recipes = new();

    public IReadOnlyList<Recipe> All => recipes;
    //mostra os crias que podem ser feitos na estação
    public IEnumerable<Recipe> ForStation(StationType stationType) => recipes.Where(r => r.stationType == StationType.Null || r.stationType == stationType);
    //verifica se pode craftar o item
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
    //retorna os recipes que podem ser craftados
    public IEnumerable<Recipe> GetCraftableRecipes(InventoryContainer inventory, StationType station) =>
        ForStation(station).Where(r => CanCraft(inventory, r, station));
}
