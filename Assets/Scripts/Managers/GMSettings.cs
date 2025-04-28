using UnityEngine;

[CreateAssetMenu(fileName = "GMSettings", menuName = "Config/GMSettings")]
public class GMSettings : ScriptableObject
{
    public GameObject canvasPrefab;
    public GameObject menuPrefabPause;
    public GameObject menuPrefabWin;
    public GameObject menuPrefabLose;
    public GameObject menuPrefabReload;
    public GameObject menuPrefabInteract;
    public GameObject menuPrefabTrap;
    public GameObject menuPrefabLock;
}
