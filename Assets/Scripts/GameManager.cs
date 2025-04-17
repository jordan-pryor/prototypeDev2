using UnityEngine;
using UnityEngine.AI;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    //[SerializeField] TMP_Text gameGoalCountText;

    //public Image playerHPBar;
    public GameObject playerDamageScreen;

    public GameObject player;
    public playerController playerScript;

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
        playerScript = player.GetComponent<playerController>();
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
                stateUnpaused();

            }


        }
    }

    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void stateUnpaused()
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
       // gameGoalCountText.text = gameGoalCount.ToString("F0");

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
