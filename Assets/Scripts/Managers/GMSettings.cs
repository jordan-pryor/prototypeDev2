using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "GMSettings", menuName = "Config/GMSettings")]
public class GMSettings : ScriptableObject
{
    public GameObject canvasPrefab;          // Root UI canvas

    public GameObject menuPrefabPause;       // Pause menu UI
    public GameObject menuPrefabWin;         // Win screen UI
    public GameObject menuPrefabLose;        // Game over screen UI
    public GameObject menuPrefabReload;      // Reload prompt

    public GameObject menuPrefabInteract;    // Interact prompt
    public GameObject menuPrefabTrap;        // Trap UI element
    public GameObject menuPrefabLock;        // Lock UI
}
