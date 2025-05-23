using System.Collections.Generic;
using UnityEngine;
using static Inventory;

public class CraftingSystem : MonoBehaviour
{
    [Header("Debug/Test Recipes")]
    [SerializeField] private CraftingRecipe[] testRecipes; // Assign recipes in inspector for testing

    private void Update()
    {
        // Temporary test key to trigger crafting
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (testRecipes.Length > 0)
            {
                TryCraft(testRecipes[0]);
            }
        }
    }

    /// <summary>
    /// Attempts to craft a recipe. Returns true if successful.
    /// </summary>
    /// <param name="recipe">The recipe to craft</param>
    /// <returns>True if crafting succeeds, false otherwise</returns>
    public bool TryCraft(CraftingRecipe recipe)
    {
        Inventory playerInventory = GameManager.instance.playerInventory;

        // Check if all ingredients are available
        foreach (var ingredient in recipe.ingredients)
        {
            if (!playerInventory.HasMaterials(ingredient.itemID, ingredient.quantity))
            {
                Debug.Log($"Missing: {ingredient.itemID} x{ingredient.quantity}");
                return false;
            }
        }

        // Consume ingredients
        foreach (var ingredient in recipe.ingredients)
        {
            playerInventory.ConsumeMaterials(ingredient.itemID, ingredient.quantity);
        }

        // Add result
        playerInventory.TryAdd(recipe.result);
        Debug.Log($"Crafted: {recipe.result.name}");
        return true;
    }
}