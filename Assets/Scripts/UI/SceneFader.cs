using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{

    public Animator animator;
    int sceneIndex;

    public void StartFadeToScene(int _sceneIndex)
    {
        sceneIndex = _sceneIndex;
        Time.timeScale = 1;
        animator.SetTrigger("FadeStart");
    }

    public void LoadScene()
    {
        if (sceneIndex != 1)
            AudioManager.instance.Stop("Background");
        if (sceneIndex != 0)
            AudioManager.instance.Stop("MainMenu");

        SceneManager.LoadScene(sceneIndex);
    }

}
