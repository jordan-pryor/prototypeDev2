namespace Crafting
{
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;

    public class CraftingRecipeUIItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI recipeNameText;
        [SerializeField] private Button recipeButton;

        public void Setup(CraftingRecipe recipe, bool canCraft, System.Action onClick)
        {
            recipeNameText.text = recipe.recipeName;
            recipeButton.interactable = canCraft;
            recipeButton.onClick.AddListener(() => onClick.Invoke());
        }
    }
}