using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingButtonUi : MonoBehaviour
{
    
    [SerializeField] private TextMeshProUGUI titleText;
    //[SerializeField] private TextMeshProUGUI ingredientsText;
    [SerializeField] private Button craftButton;

    public void Configure(Recipe recipe, Action onCLick)
    {
        if(recipe == null) return;

        titleText.text = recipe.resultItem.itemName;

        craftButton.onClick.RemoveAllListeners();
        if(onCLick != null)
            craftButton.onClick.AddListener(() => onCLick.Invoke());
    }
}
