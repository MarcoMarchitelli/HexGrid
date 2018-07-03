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
        AudioManager.instance.Stop("Background");
        AudioManager.instance.Stop("MainMenu");
        AudioManager.instance.Stop("Victory");

        SceneManager.LoadScene(sceneIndex);
    }

}
