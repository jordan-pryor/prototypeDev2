using System.Collections.Generic;
using UnityEngine;

public class CraftingTable : MonoBehaviour, IInteract
{
    public List<CraftingRecipe> availableRecipes;
    public void Interact()
    {
        GameManager.instance.OpenCrafting(this);
    }
}
