using UnityEngine;
using UnityEngine.SceneManagement;

/// Handles button UI functionality like resume, restart, and quit.
public class buttonFunction : MonoBehaviour
{
    /// Resumes the game from a paused state.
    public void Resume()
    {
        gameManager.instance.stateUnpause();
    }


    /// Restarts the current scene and unpauses the game.

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.stateUnpause();
    }

    /// Quits the application (or stops play mode if in editor).
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
