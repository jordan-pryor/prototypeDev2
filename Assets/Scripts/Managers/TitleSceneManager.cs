using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{

    public string gameSceneStart = "Quarantine Block";
    public GameObject startPanel;
    public GameObject optionsPanel;


    

    public void OnStartPressed()
    {
        SceneManager.LoadScene(1);
    }
    
    public void OnOptionsPressed()
    {
        startPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void OnBackfromOptions()
    {
        optionsPanel.SetActive(false);
        startPanel.SetActive(true);
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
