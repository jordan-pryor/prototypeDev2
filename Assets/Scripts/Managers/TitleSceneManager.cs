using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{

    public string gameSceneStart = "Quarantine Block";
    public GameObject optionsPanel;
    public GameObject player;
    public MonoBehaviour movementScript;  //were playerController goes.


    private void Awake()
    {
        if (movementScript != null)
            movementScript.enabled = false;
    }

    public void OnStartPressed()
    {
        if (movementScript != null)
            movementScript.enabled = true;

        SceneManager.LoadScene(gameSceneStart);
    }
    
    public void OnOptionsPressed()
    {
        optionsPanel.SetActive(true);
    }

    public void OnQuitPressed()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
