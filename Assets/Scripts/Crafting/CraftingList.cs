using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingList : MonoBehaviour
{
    [SerializeField] GameObject entryPrefab;
    [SerializeField] Transform contentParent;
    private List<CraftingRecipe> recipes;
    public void StoreRecipes(List<CraftingRecipe> recIn)
    {
        recipes = recIn;
        RefreshDisplay();
    }
    private void RefreshDisplay()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
        List<CraftingRecipe> craftable = new List<CraftingRecipe>();
        List<CraftingRecipe> notCraftable = new List<CraftingRecipe>();

        foreach (CraftingRecipe recipe in recipes)
        {
            if (IsCraftable(recipe))
                craftable.Add(recipe);
            else
                notCraftable.Add(recipe);
        }

        foreach (CraftingRecipe recipe in craftable)
            CreateEntry(recipe, true);

        foreach (CraftingRecipe recipe in notCraftable)
            CreateEntry(recipe, false);
    }

    private bool IsCraftable(CraftingRecipe recipe)
    {
        foreach (var ing in recipe.ingredients)
        {
            if (!GameManager.instance.playerInventory.HasMaterials(ing.itemID, ing.quantity))
                return false;
        }
        return true;
    }
    private void CreateEntry(CraftingRecipe recipe, bool canCraft)
    {
        GameObject entry = Instantiate(entryPrefab, contentParent);
        TMP_Text[] texts = entry.GetComponentsInChildren<TMP_Text>();
        TMP_Text nameText = texts.First(t => t.name == "Name");
        TMP_Text ingredientsText = texts.First(t => t.name == "Ingredients");
        TMP_Text resultText = texts.First(t => t.name == "Result");
        Button craftButton = entry.GetComponentInChildren<Button>();
        nameText.text = recipe.recipeName;
        List<string> ingList = new List<string>();
        foreach (var ing in recipe.ingredients)
            ingList.Add($"{ing.itemID} (x{ing.quantity})");
        ingredientsText.text = string.Join(" + ", ingList);

        resultText.text = recipe.result.itemName;
        Color textColor = canCraft ? Color.white : Color.gray;
        nameText.color = ingredientsText.color = resultText.color = textColor;
        craftButton.interactable = canCraft;
        craftButton.onClick.RemoveAllListeners();
        if (canCraft)
            craftButton.onClick.AddListener(() => Craft(recipe));
    }
    public void Craft(CraftingRecipe recipe)
    {
        foreach (var ingredient in recipe.ingredients)
        {
            GameManager.instance.playerInventory.ConsumeMaterials(ingredient.itemID, ingredient.quantity);
        }
        GameManager.instance.playerInventory.TryAdd(recipe.result);
        RefreshDisplay();
    }
}
