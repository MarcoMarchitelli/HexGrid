using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

    private void Start()
    {
        AudioManager.instance.Play("MainMenu");
    }

    public void Play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Options()
    {
        //xd
    }

    public void Quit()
    {
        Application.Quit();
    }
}
