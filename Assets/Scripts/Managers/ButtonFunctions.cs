using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunction : MonoBehaviour
{
    // Resume the game from pause
    public void resume()
    {
        GameManager.instance.stateUnpause();
    }

    // Restart the current scene and unpause the game
    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.instance.stateUnpause();
    }
    public void exitCraft()
    {
        GameManager.instance.stateUnpause();
    }

    // Quit the game (works in both editor and build)
    public void quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in editor
#else
        Application.Quit(); // Quit standalone application
#endif
    }
}
