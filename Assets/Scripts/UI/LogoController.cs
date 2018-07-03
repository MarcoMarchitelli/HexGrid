using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoController : MonoBehaviour {

    public SceneFader sceneFader;
    public float TimeToFade = 3f;

    float timeCounter = 0;
    bool isFading = false;
	
	// Update is called once per frame
	void Update () {

        timeCounter += Time.deltaTime;

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return)) && !isFading)
        {
            sceneFader.StartFadeToScene(SceneManager.GetActiveScene().buildIndex + 1);
            isFading = true;
        }

        if(timeCounter >= TimeToFade && !isFading)
        {
            sceneFader.StartFadeToScene(SceneManager.GetActiveScene().buildIndex + 1);
            isFading = true;
        }
            
	}
}
