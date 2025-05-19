using UnityEngine;

public class PlayerCrafting : MonoBehaviour
{
	[SerializeField] Inventory inventory;
	[SerializeField] CraftingRecipe[] testRecipes; // Assign in Inspector

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.C)) // Example: Craft with key press
		{
			if (testRecipes.Length > 0)
			{
				TryCraft(testRecipes[0]); // Try to craft the first recipe
			}
		}
	}

	public bool TryCraft(CraftingRecipe recipe)
	{
		foreach (var ingredient in recipe.ingredients)
		{
			if (!inventory.HasMaterials(ingredient.itemID, ingredient.quantity))
			{
				Debug.Log("Missing ingredient: " + ingredient.itemID);
				return false;
			}
		}

		foreach (var ingredient in recipe.ingredients)
		{
			inventory.ConsumeMaterials(ingredient.itemID, ingredient.quantity);
		}

		inventory.TryAdd(recipe.result);
		Debug.Log("Crafted: " + recipe.result.name);
		return true;
	}
}
