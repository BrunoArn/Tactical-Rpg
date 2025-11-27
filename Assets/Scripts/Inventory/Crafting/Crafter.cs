using UnityEngine;

public class Crafter : MonoBehaviour
{
    [SerializeField] private RecipeDatabase recipeDatabase;
    [SerializeField] private InventoryManager inventoryManager; //backpack + quickbar
    [SerializeField] private StationType currentStationType = StationType.Null;

    [SerializeField] private GameEvent redrawUIEvent;

    public bool TryCraft(Recipe recipe)
    {
        if(inventoryManager == null || recipeDatabase == null || recipe == null) return false;

        if(!recipeDatabase.CanCraft(inventoryManager, recipe, currentStationType))
        {
            Debug.Log("Cannot craft recipe: " + recipe.name);
            return false;
        } 

        foreach(var ingredient in recipe.ingredients) inventoryManager.RemoveItem(ingredient.item, ingredient.quantity);

        if(!inventoryManager.AddItem(recipe.resultItem, recipe.resultQuantity)) return false;

        
        return true;
    }

    [ContextMenu("Criar primeira Recipe")]
    public void EquipWeapon()
    {
        TryCraft(recipeDatabase.All[0]);
        redrawUIEvent.Raise();
    }
}
