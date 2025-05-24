using UnityEngine;

[CreateAssetMenu(fileName = "CraftingRecipe", menuName = "Scriptable Objects/CraftingRecipe")]
public class CraftingRecipe : ScriptableObject
{
	public string recipeName;
	public Ingredient[] ingredients  ;
	public BaseData result;
}

[System.Serializable]
public class Ingredient
{
	public string itemID;
	public int quantity;
}