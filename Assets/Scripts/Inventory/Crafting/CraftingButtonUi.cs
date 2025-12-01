using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Small UI helper that configures a single crafting button entry.
///
/// Usage:
/// - Call <see cref="Configure(Recipe, Action)"/> with the recipe and a click callback
///   to set the label and bind the button.
/// - The component does not perform crafting itself; it only updates the button
///   and title text UI.
/// </summary>
public class CraftingButtonUi : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Image iconImage;
    // TODO: Consider showing the recipe's ingredients text using this field
    // [SerializeField] private TextMeshProUGUI ingredientsText;
    [SerializeField] private Button craftButton;

    public void Configure(Recipe recipe, Action onCLick)
    {
        if (recipe == null) return;

        // show the resulting item's name for clarity
        titleText.text = recipe.resultItem.itemName;
        iconImage.sprite = recipe.resultItem.icon;

        // make sure only the fresh click handler is attached
        craftButton.onClick.RemoveAllListeners();
        if (onCLick != null)
            craftButton.onClick.AddListener(() => onCLick.Invoke());
    }
}
