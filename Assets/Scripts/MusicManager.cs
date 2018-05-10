using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    public AudioClip BGMusic;
    public AudioClip BetMusic;

    private void Start()
    {
        SoundManager.instance.PlayMusic(BGMusic, 2);

    }


   // void Update () {
   //     if (Input.GetKeyDown(KeyCode.Space))
     //   {
       //     SoundManager.instance.PlayMusic(BGMusic, 3);
        //}
	//}
}
