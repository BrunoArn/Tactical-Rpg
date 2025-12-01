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
    /// <summary>
    /// Title text used to show the result item's name.
    /// </summary>
    [SerializeField] private TextMeshProUGUI titleText;

    // TODO: Consider showing the recipe's ingredients text using this field
    // [SerializeField] private TextMeshProUGUI ingredientsText;

    /// <summary>
    /// The actual button the player clicks to perform crafting.
    /// </summary>
    [SerializeField] private Button craftButton;

    /// <summary>
    /// Configure the UI for a specific <paramref name="recipe"/>.
    /// The supplied <paramref name="onCLick"/> callback will be invoked when the
    /// player presses the configured button.
    /// </summary>
    /// <remarks>
    /// This method clears any existing listeners on the button before adding the
    /// new one. If <paramref name="recipe"/> is null the method returns early
    /// and does not modify the button.
    /// </remarks>
    public void Configure(Recipe recipe, Action onCLick)
    {
        if (recipe == null) return;

        // show the resulting item's name for clarity
        titleText.text = recipe.resultItem.itemName;

        // make sure only the fresh click handler is attached
        craftButton.onClick.RemoveAllListeners();
        if (onCLick != null)
            craftButton.onClick.AddListener(() => onCLick.Invoke());
    }
}
