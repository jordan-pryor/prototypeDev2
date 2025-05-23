using UnityEngine;
using TMPro;

    namespace Crafting
    {
        public class CraftingMaterials : MonoBehaviour
        {
            [Header("UI Components")]
            [SerializeField] private TextMeshProUGUI materialNameText;
            [SerializeField] private TextMeshProUGUI materialAmountText;

            private Inventory playerInventory;

            private string currentMaterialName;

            private void Awake()
            {
                // Assume the player Inventory is on a GameObject tagged "Player"
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerInventory = player.GetComponent<Inventory>();
                }
                else
                {
                    Debug.LogWarning("Player object with Inventory not found.");
                }
            }
            public void Setup(string materialName, int currentAmount)
            {
                currentMaterialName = materialName;
                materialNameText.text = materialName;
                materialAmountText.text = currentAmount.ToString();
                materialAmountText.color = Color.white; // default color for inventory display
            }
            /// <summary>
            /// Sets up the material display in the materials panel.
            /// </summary>
            /// <param name="materialName">Name of the material</param>
            /// <param name="ingredientQuantity"></param>
            /// <param name="requiredQuantity"></param>
            /// <param name="hasEnough"></param>
            public void Setup(string materialName, int ingredientQuantity, int requiredQuantity, bool hasEnough)
            {
                currentMaterialName = materialName;
                materialNameText.text = materialName;
                UpdateAmount();

                // Optional: change amount text color based on hasEnough
                materialAmountText.text = $"{requiredQuantity}";
                materialAmountText.color = hasEnough ? Color.white : new Color(0.5f, 0.5f, 0.5f); // Greyed Out when Not available 

            }

            /// <summary>
            /// Updates the amount text by querying the player's inventory.
            /// </summary>
            public void UpdateAmount()
            {
                if (playerInventory == null)
                {
                    materialAmountText.text = "0";
                    return;
                }

                int amount = 0;
                foreach (var material in playerInventory.materials)
                {
                    if (material.name == currentMaterialName)
                    {
                        amount = material.amount;
                        break;
                    }
                }
                materialAmountText.text = amount.ToString();
            }
        }
    }