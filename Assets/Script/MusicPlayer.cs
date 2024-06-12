using Container;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource audioSourceForSE;
    [SerializeField] private AudioContainer audioContainer;
    private bool startFlag;
    private bool loopFlag;
    private NameBGM nameLoopBGM;

    private void Update()
    {
        if (!audioSource.loop && !audioSource.isPlaying && loopFlag && startFlag)
        {
            audioSource.clip = audioContainer.GetBGM(nameLoopBGM);
            audioSource.Play();
            audioSource.loop = true;
        }
    }

    public void SetMusicAndPlay(NameBGM introBGM, NameBGM loopBGM)
    {
        audioSource.Stop();
        audioSource.clip = audioContainer.GetBGM(introBGM);
        audioSource.Play();
        audioSource.loop = false;
        loopFlag = true;
        nameLoopBGM = loopBGM;
        startFlag = true;
    }
    
    public void SetMusicAndPlay(NameBGM nameBGM, bool isLoop)
    {
        audioSource.Stop();
        audioSource.clip = audioContainer.GetBGM(nameBGM);
        audioSource.Play();
        loopFlag = isLoop;
        audioSource.loop = isLoop;
        startFlag = true;
    }

    public void Stop()
    {
        audioSource.Stop();
        startFlag = false;
    }

    public void PlayOneShot(AudioClip audioClip)
    {
        audioSourceForSE.PlayOneShot(audioClip);
    }
}
