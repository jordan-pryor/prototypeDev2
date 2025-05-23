using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Crafting
{
    public class CraftingUI : MonoBehaviour
{
    // ───── UI References ─────
    [Header("UI References")]
    [SerializeField] private GameObject craftingPanel; // 👈 Root crafting panel that toggles UI visibility
    [SerializeField] private Transform recipeListParent; // 👈 UI parent container for recipe buttons/items
    [SerializeField] private GameObject recipeItemPrefab; // 👈 Prefab used to visually represent each recipe in list
    [SerializeField] private Button closeButton; // 👈 Button that closes the crafting menu

    // ───── Recipe Details Panel ─────
    [Header("Recipe Details Panel")]
    [SerializeField] private GameObject recipeDetailsPanel; // 👈 Displays ingredients and output for selected recipe
    [SerializeField] private TextMeshProUGUI recipeNameText; // 👈 Text element showing the selected recipe's name
    [SerializeField] private Transform ingredientsListParent; // 👈 UI parent container for ingredient list
    [SerializeField] private GameObject ingredientItemPrefab; // 👈 Prefab used to visually show each ingredient
    [SerializeField] private Button craftButton; // 👈 Button used to confirm crafting the selected item
    [SerializeField] private Image resultItemIcon; // 👈 Image display for the crafted item (disabled for now)
    [SerializeField] private TextMeshProUGUI resultItemNameText; // 👈 Text showing the result item's name

    // ───── Materials Inventory Panel ─────
    [Header("Materials Inventory Panel")]
    [SerializeField] private GameObject materialsPanel; // 👈 Panel that shows the player's material inventory
    [SerializeField] private Transform materialsListParent; // 👈 UI parent container for material list items
    [SerializeField] private GameObject materialItemPrefab; // 👈 Prefab used for each material displayed
    [SerializeField] private Button toggleMaterialsButton; // 👈 Button to show/hide the materials panel

    // ───── Runtime Data ─────
    private CraftingRecipe[] availableRecipes; // 👈 Array of available recipes passed in from gameplay
    private CraftingRecipe selectedRecipe; // 👈 Currently selected recipe for detail/craft view
    private List<GameObject> recipeUIItems = new(); // 👈 Instantiated UI objects for recipe list
    private List<GameObject> ingredientUIItems = new(); // 👈 Instantiated UI objects for ingredient list
    private List<GameObject> materialUIItems = new(); // 👈 Instantiated UI objects for material list
    private bool materialsVisible = false; // 👈 Toggle state of materials panel visibility

    private void Start()
    {
        SetupUI(); // 👈 Assign all UI button listeners
        craftingPanel.SetActive(false); // 👈 Hide UI by default
    }

    private void SetupUI()
    {
        closeButton.onClick.AddListener(CloseCraftingUI);
        craftButton.onClick.AddListener(CraftSelectedItem);
        toggleMaterialsButton.onClick.AddListener(ToggleMaterialsPanel);

        recipeDetailsPanel.SetActive(false);
        materialsPanel.SetActive(false);
    }

    // Called when the player opens the crafting interface
    public void OpenCraftingUI(CraftingRecipe[] recipes)
    {
        availableRecipes = recipes;
        craftingPanel.SetActive(true);

        GameManager.instance.isPaused = true; // Pause game logic
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Disable player interaction while crafting
        var interact = GameManager.instance?.player.GetComponent<playerInteract>();
        if (interact != null)
        {
            interact.enabled = false;
        }

        PopulateRecipeList(); // Build UI recipe list
        UpdateMaterialsList(); // Populate material panel (if visible)
    }

    // Close the crafting interface and restore game state
    public void CloseCraftingUI()
    {
        craftingPanel.SetActive(false);
        GameManager.instance.isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        var interact = GameManager.instance?.player.GetComponent<playerInteract>();
        if (interact != null)
        {
            interact.enabled = true;
        }

        ClearUILists(); // Cleanup old UI elements
        selectedRecipe = null;
        recipeDetailsPanel.SetActive(false);
    }

    // Populate the crafting recipe list from availableRecipes
    private void PopulateRecipeList()
    {
        ClearRecipeList();

        foreach (var recipe in availableRecipes)
        {
            GameObject recipeItem = Instantiate(recipeItemPrefab, recipeListParent);
            var recipeUI = recipeItem.GetComponent<CraftingRecipeUIItem>();

            if (recipeUI != null)
            {
                bool canCraft = CanCraftRecipe(recipe);
                recipeUI.Setup(recipe, canCraft, () => SelectRecipe(recipe));
                recipeUIItems.Add(recipeItem);
            }
        }
    }

    // Called when a player clicks a recipe in the list
    private void SelectRecipe(CraftingRecipe recipe)
    {
        selectedRecipe = recipe;
        ShowRecipeDetails(recipe);
    }

    // Show selected recipe details and check player inventory
    private void ShowRecipeDetails(CraftingRecipe recipe)
    {
        recipeDetailsPanel.SetActive(true);
        recipeNameText.text = recipe.recipeName;

        if (recipe.result != null)
        {
            resultItemNameText.text = recipe.result.itemName;
            resultItemIcon.enabled = false; // Placeholder for item icon system
        }

        ClearIngredientsList();

        foreach (var ingredient in recipe.ingredients)
        {
            GameObject ingredientItem = Instantiate(ingredientItemPrefab, ingredientsListParent);
            var ingredientComponent = ingredientItem.GetComponent<CraftingMaterials>();

            if (ingredientComponent != null)
            {
                int playerAmount = GetMaterialAmount(ingredient.itemID);
                bool hasEnough = playerAmount >= ingredient.quantity;

                ingredientComponent.Setup(ingredient.itemID, ingredient.quantity, playerAmount, hasEnough);
                ingredientUIItems.Add(ingredientItem);
            }
        }

        bool canCraft = CanCraftRecipe(recipe);
        craftButton.interactable = canCraft;
        craftButton.GetComponentInChildren<TextMeshProUGUI>().text = canCraft ? "CRAFT" : "MISSING MATERIALS";
    }

    // Attempt to craft the currently selected recipe
    private void CraftSelectedItem()
    {
        if (selectedRecipe == null) return;

        bool success = GameManager.instance.playerInventory.GetComponent<PlayerCrafting>().TryCraft(selectedRecipe);

        if (success)
        {
            PopulateRecipeList(); // Refresh recipe availability
            ShowRecipeDetails(selectedRecipe); // Refresh ingredient counts
            UpdateMaterialsList(); // Refresh material panel

            Debug.Log($"Successfully crafted {selectedRecipe.recipeName}!");
        }
        else
        {
            Debug.Log("Failed to craft item!");
        }
    }

    // Toggle the visibility of the player's material inventory panel
    private void ToggleMaterialsPanel()
    {
        materialsVisible = !materialsVisible;
        materialsPanel.SetActive(materialsVisible);

        if (materialsVisible)
            UpdateMaterialsList();
    }

    // Refresh the list of player materials shown in the inventory panel
    private void UpdateMaterialsList()
    {
        if (!materialsVisible) return;

        ClearMaterialsList();

        var inventory = GameManager.instance.playerInventory;
        foreach (var material in inventory.materials)
        {
            if (material.amount > 0)
            {
                GameObject materialItem = Instantiate(materialItemPrefab, materialsListParent);
                var materialComponent = materialItem.GetComponent<CraftingMaterials>();

                if (materialComponent != null)
                {
                    // Only pass the material name; amount is fetched internally by CraftingMaterials
                    materialComponent.Setup(material.name, material.amount);
                    materialUIItems.Add(materialItem);
                }
            }
        }
    }

    // Determine if player has enough materials to craft a recipe
    private bool CanCraftRecipe(CraftingRecipe recipe)
    {
        foreach (var ingredient in recipe.ingredients)
        {
            if (!GameManager.instance.playerInventory.HasMaterials(ingredient.itemID, ingredient.quantity))
                return false;
        }
        return true;
    }

    // Helper method to get material count from inventory
    private int GetMaterialAmount(string itemID)
    {
        foreach (var material in GameManager.instance.playerInventory.materials)
        {
            if (material.name == itemID)
                return material.amount;
        }
        return 0;
    }

    // ───── Cleanup ─────

    // Destroy all UI lists before refreshing
    private void ClearUILists()
    {
        ClearRecipeList();
        ClearIngredientsList();
        ClearMaterialsList();
    }

    // Clear recipe UI entries
    private void ClearRecipeList()
    {
        foreach (var item in recipeUIItems)
            if (item != null) Destroy(item);
        recipeUIItems.Clear();
    }

    // Clear ingredient UI entries
    private void ClearIngredientsList()
    {
        foreach (var item in ingredientUIItems)
            if (item != null) Destroy(item);
        ingredientUIItems.Clear();
    }

    // Clear material UI entries
    private void ClearMaterialsList()
    {
        foreach (var item in materialUIItems)
            if (item != null) Destroy(item);
        materialUIItems.Clear();
    }
}
}