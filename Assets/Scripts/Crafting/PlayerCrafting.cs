using System.Collections.Generic;
using UnityEngine;
using static Inventory;

public class PlayerCrafting : MonoBehaviour
{
    [SerializeField] private CraftingRecipe[] testRecipes;       // Assign recipes in inspector

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (testRecipes.Length > 0)
            {
                TryCraft(testRecipes[0]);
            }
        }
    }

    public bool TryCraft(CraftingRecipe recipe)
    {
        // Check if all ingredients are available
        foreach (var ingredient in recipe.ingredients)
        {
            if (!GameManager.instance.playerInventory.HasMaterials(ingredient.itemID, ingredient.quantity))
            {
                return false;
            }
        }

        // Consume ingredients
        foreach (var ingredient in recipe.ingredients)
        {
            GameManager.instance.playerInventory.ConsumeMaterials(ingredient.itemID, ingredient.quantity);
        }

        // Add the result
        GameManager.instance.playerInventory.TryAdd(recipe.result);
        return true;
    }
}
