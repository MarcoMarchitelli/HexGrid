using UnityEngine;

public class AudioManager : MonoBehaviour {

    [Header("Audio Sources")]
    public AudioSource Background;
    public AudioSource Victory;
    public AudioSource FightStart;
    public AudioSource FightEndAtkWin;
    public AudioSource FightEndDefWin;

    public void Start()
    {
        Background.Play();
    }

    public void Win()
    {

    }

}
