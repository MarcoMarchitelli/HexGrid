using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    float masterVolumePercent = 1;
    float sfxVolumePercent = 1;
    float musicVolumePercent = 1;

    AudioSource[] musicSources;
    int activeMusicScorceIndex;

    public static SoundManager instance;

    private void Awake()
    {

        instance = this;

        musicSources = new AudioSource[2];
        for(int i= 0; i<2; i++)
        {
            GameObject newMusicSource = new GameObject("Music source" + (i + 1));
            musicSources[i] = newMusicSource.AddComponent<AudioSource>();
            newMusicSource.transform.parent = transform;
        }
    }

    public void PlayMusic(AudioClip clip, float fadeDuration= 1)
    {
        activeMusicScorceIndex = 1 - activeMusicScorceIndex;
        musicSources[activeMusicScorceIndex].clip = clip;
        musicSources[activeMusicScorceIndex].Play();

        StartCoroutine(AnimateMusicCrossfade(fadeDuration));
    }

    public  void PlaySound(AudioClip clip, Vector3 position)
    {
        AudioSource.PlayClipAtPoint(clip, position, sfxVolumePercent*masterVolumePercent);
    }
    
    IEnumerator AnimateMusicCrossfade (float duration)
    {
        float percent =0;
        while(percent < 1)
        {
            percent += Time.deltaTime * 1 / duration;
            musicSources[activeMusicScorceIndex].volume = Mathf.Lerp(0, musicVolumePercent * masterVolumePercent, percent);
            musicSources[1-activeMusicScorceIndex].volume = Mathf.Lerp( musicVolumePercent * masterVolumePercent,0, percent);
            yield return null;

        }

    }
}
