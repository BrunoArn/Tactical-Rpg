using UnityEngine;

public enum StationType { Null, ArmorStation, WeaponStation, BenchStation }

[CreateAssetMenu(fileName = "Recipe", menuName = "Items/Recipe")]
public class Recipe : ScriptableObject
{
    [System.Serializable]
    public struct Ingredient
    {
        public ItemData item;
        public int quantity;
    }

    public Ingredient[] ingredients;
    public ItemData resultItem;
    public int resultQuantity = 1;
    public StationType stationType;
}
