using System.Collections.Generic;
using UnityEngine;
using static Inventory;

public class CraftingSystem : MonoBehaviour
{
    [Header("Debug/Test Recipes")] [SerializeField]
    private CraftingRecipe[] testRecipes; // Assign recipes in inspector for testing

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
        // Check if all ingredients are available
        foreach (var ingredient in recipe.ingredients)
        {
            if (!GameManager.instance.playerInventory.HasMaterials(ingredient.itemID, ingredient.quantity))
                return false;
        }

        // Consume ingredients
        foreach (var ingredient in recipe.ingredients)
        {
            GameManager.instance.playerInventory.ConsumeMaterials(ingredient.itemID, ingredient.quantity);
        }

        // Try to add crafted item
        bool added = GameManager.instance.playerInventory.TryAdd(recipe.result);
        if (!added)
        {
            // Inventory full — drop item in the world
            DropItemInWorld(item: ItemPickup.Instantiate(recipe.result));
        }

        return true;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void DropItemInWorld(BaseData item)
    {
        GameObject droppedItemPrefab = item.prefab; // You must assign this prefab to the Item
        if (droppedItemPrefab)
        {
            Vector3 dropPosition = transform.position + transform.forward * 1.5f;
            Instantiate(droppedItemPrefab, dropPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning($"No world prefab assigned for item: {item.name}");
        }
    }
}