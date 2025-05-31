using UnityEngine;
using UnityEngine.AI;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Collections;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] GMSettings settings;     // UI configuration
    Canvas UICanvas;

    GameObject menuPause;
    GameObject menuActive;
    GameObject menuWin;
    GameObject menuLose;

    public GameObject promptTrap;             // UI: trap warning
    public GameObject promptInteract;         // UI: interact prompt
    public GameObject promptReload;           // UI: reload prompt
    public GameObject promptLock;             // UI: locked interaction
    public GameObject inventory;            // UI: Inventory
    public GameObject crafting;            // UI: crafting

    public TMP_Text gameGoalCountText;        // Text to display goal progress
    public GameObject playerDamageScreen;     // Red flash or feedback when damaged
    public GameObject player;                 // Player reference
    public PlayerController playerController; // Reference to player controller script
    public Inventory playerInventory;         // Reference to Inventory
    
    public bool killEnemies = true;           // Enables win condition check
    public bool isPaused;                     // Pause state flag
    private bool inventoryOpen = false;
    float timeScaleOrig;                      // Used to restore time scale on unpause
    int gameGoalCount;                        // Tracks how many goals remain

    void Awake()
    {
        instance = this;

        // Cache player references
        player = GameObject.FindWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        playerInventory = player.GetComponent<Inventory>();

        // Lock cursor for gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Setup UI from settings
        UICanvas = Instantiate(settings.canvasPrefab).GetComponent<Canvas>();
        menuPause = Instantiate(settings.menuPrefabPause, UICanvas.transform);
        menuWin = Instantiate(settings.menuPrefabWin, UICanvas.transform);
        menuLose = Instantiate(settings.menuPrefabLose, UICanvas.transform);
        promptInteract = Instantiate(settings.menuPrefabInteract, UICanvas.transform);
        promptReload = Instantiate(settings.menuPrefabReload, UICanvas.transform);
        promptTrap = Instantiate(settings.menuPrefabTrap, UICanvas.transform);
        promptLock = Instantiate(settings.menuPrefabLock, UICanvas.transform);
        inventory = Instantiate(settings.menuPrefabInventory, UICanvas.transform);
        crafting = Instantiate(settings.menuPrefabCrafting, UICanvas.transform);
        // Disable menus initially
        menuPause.SetActive(false);
        menuWin.SetActive(false);
        menuLose.SetActive(false);
        inventory.SetActive(false);
        crafting.SetActive(false);
        HidePrompts();

        timeScaleOrig = Time.timeScale;
    }

    void Update()
    {
        // Handle pause input (usually ESC)
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuPause.SetActive(true);
            }
            else if (menuActive == menuPause)
            {
                stateUnpause();
            }
        }

        if (Input.GetButtonDown("Inventory"))
        {
            inventoryOpen = !inventoryOpen;
        }
    }

    // Hides all in-game interaction prompts
    public void HidePrompts()
    {
        promptInteract.SetActive(false);
        promptReload.SetActive(false);
        promptTrap.SetActive(false);
        promptLock.SetActive(false);
    }

    // Pauses the game and shows cursor
    public void statePause()
    {
        HidePrompts();
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Unpauses the game and hides cursor
    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    // Updates the game goal counter and checks win condition
    public void updateGameGoal(int amount)
    {
        gameGoalCount += amount;
        playerController.goalText.text = gameGoalCount.ToString("F0");

        if (gameGoalCount <= 0 && killEnemies)
        {
            youWin();
        }
    }

    // Called when win condition is met
    public void youWin()
    {
        statePause();
        menuActive = menuWin;
        menuActive.SetActive(true);
    }

    // Called when player loses the game
    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    // Shows a short lock interaction prompt
    public void LockPrompt()
    {
        StartCoroutine(LockPromptRoutine(1.0f));
    }

    // Coroutine for timed display of lock message
    private IEnumerator LockPromptRoutine(float duration)
    {
        promptLock.SetActive(true);
        yield return new WaitForSeconds(duration);
        promptLock.SetActive(false);
    }

    // Turn inventory on and off.
    public void ToggleInventory()
    {
        if(inventoryOpen)
        {
            statePause();
            inventory.SetActive(true);
        }
        else
        {
            stateUnpause();
            inventory.SetActive(false);
        }
    }

    public void OpenCrafting(CraftingTable craftingTable)
    {
        statePause();
        menuActive = crafting;
        crafting.GetComponent<CraftingList>().StoreRecipes(craftingTable.availableRecipes);
        menuActive.SetActive(true);
    }
}
