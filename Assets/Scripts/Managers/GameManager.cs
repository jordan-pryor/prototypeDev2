using UnityEngine;
using UnityEngine.AI;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] GMSettings settings;

    Canvas UICanvas;

    GameObject menuPause;
    GameObject menuActive;
    GameObject menuWin;
    GameObject menuLose;

    public GameObject promptTrap;
    public GameObject promptInteract;
    public GameObject promptReload;

    [SerializeField] TMP_Text gameGoalCountText;
    public Image playerHPBar;
    public GameObject playerDamageScreen;

    public GameObject player;
    public PlayerController playerController;

    public bool isPaused;
    float timeScaleOrig;
    int gameGoalCount;
    // possible future use with enemies for tracking.
    //public List<EnemyAI> allEnemies = new List<EnemyAI>();

    // Awake is called before start. 
    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        UICanvas = Instantiate(settings.canvasPrefab).GetComponent<Canvas>();
        menuPause = Instantiate(settings.menuPrefabPause, UICanvas.transform);
        menuWin = Instantiate(settings.menuPrefabWin, UICanvas.transform);
        menuLose = Instantiate(settings.menuPrefabLose, UICanvas.transform);
        promptInteract = Instantiate(settings.menuPrefabInteract, UICanvas.transform);
        promptReload = Instantiate(settings.menuPrefabReload, UICanvas.transform);
        promptTrap = Instantiate(settings.menuPrefabTrap, UICanvas.transform);

        menuPause.SetActive(false);
        menuWin.SetActive(false);
        menuLose.SetActive(false);
        promptInteract.SetActive(false);
        promptReload.SetActive(false);
        promptTrap.SetActive(false);

        timeScaleOrig = Time.timeScale;        
    }

    // Update is called once per frame
    void Update()
    {
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
    }

    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        promptInteract.SetActive(false);
        promptReload.SetActive(false);
        promptTrap.SetActive(false);
    }

    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    public void updateGameGoal(int amount)
    {
        gameGoalCount += amount;
        gameGoalCountText.text = gameGoalCount.ToString("F0");

        if (gameGoalCount <= 0)
        {
            //You Won!
            statePause();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }
    }

    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    //public void RegisterEnemy(EnemyAI enemy)
    //{
    //     allEnemies.Add(enemy);
    //}

    //public void UnregisterEnemy(EnemyAI enemy)
    //{
    //    allEnemies.Remove(enemy);
    //}

}
