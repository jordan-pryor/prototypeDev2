using UnityEngine;

public class gameManager : MonoBehaviour
{
    // === Singleton Setup ===
    public static gameManager instance;     // So everyone can find us without shouting across the room

    // === Menus ===
    [SerializeField] GameObject menuActive; // Tracks which menu is up right now
    [SerializeField] GameObject menuPause;  // Pause screen – for when players need a breather
    [SerializeField] GameObject menuWin;    // Victory screen – show them who's boss
    [SerializeField] GameObject menuLose;   // Game over screen – cue sad music

    // === Player References ===
    public GameObject player;               // Our beloved protagonist
    public playerController playerScript;   // Their brain and reflexes

    // === Game State ===
    public bool isPaused;                   // Are we frozen in time?

    // === Internals ===
    float timeScaleOrig;                    // For restoring time after a pause
    int gameGoalCount;                      // How many enemies are left (or other win condition)

    // Runs before anything else – perfect for setup
    void Awake()
    {
        instance = this;                                            // Set ourselves as the one and only
        player = GameObject.FindWithTag("Player");                  // Find our player hero
        playerScript = player.GetComponent<playerController>();     // Grab their controller script

        timeScaleOrig = Time.timeScale;                             // Remember how fast time normally moves
    }

    // Main game loop – listens for input and state changes
    void Update()
    {
        // If the player hits the "Cancel" button (usually Escape)
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                statePause();                   // Freeze the world
                menuActive = menuPause;         // Set the current menu to Pause
                menuPause.SetActive(true);      // Show it
            }
            else if (menuActive == menuPause)
            {
                stateUnpause();                 // Resume the action
            }
        }
    }

    // === State Management ===

    public void statePause()
    {
        isPaused = !isPaused;                           // Toggle pause flag
        Time.timeScale = 0;                             // Freeze everything
        Cursor.visible = true;                          // Give the player control of the cursor
        Cursor.lockState = CursorLockMode.None;         // Unlock it so they can click menus
    }

    public void stateUnpause()
    {
        isPaused = !isPaused;                           // Back to real-time
        Time.timeScale = timeScaleOrig;                 // Restore original timescale
        Cursor.visible = false;                         // Hide the cursor for immersion
        Cursor.lockState = CursorLockMode.Locked;       // Lock it to the screen center again

        menuActive.SetActive(false);                    // Close current menu
        menuActive = null;                              // Clear the pointer
    }

    // === Win/Lose Logic ===

    public void updateGameGoal(int amount)
    {
        gameGoalCount += amount;                        // Add or remove from goal count

        if (gameGoalCount <= 0)                         // All done? Victory time
        {
            statePause();                               // Stop the game
            menuActive = menuWin;
            menuActive.SetActive(true);                 // Cue the fireworks
        }
    }

    public void youLose()
    {
        statePause();                                   // Game over, man
        menuActive = menuLose;
        menuActive.SetActive(true);                     // Show the fail screen
    }
}
