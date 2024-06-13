using Container;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource[] audioSource;
    [SerializeField] private AudioSource audioSourceForSE;
    [SerializeField] private AudioContainer audioContainer;
    private int manageSourceIndex = 0;
    private int unmanageSourceIndex = 1;
    private bool startFlag;
    private bool loopFlag;
    private NameBGM nameLoopBGM;

    private void Update()
    {
        if (!audioSource[manageSourceIndex].loop && !audioSource[manageSourceIndex].isPlaying && loopFlag && startFlag)
        {
            audioSource[manageSourceIndex].clip = audioContainer.GetBGM(nameLoopBGM);
            audioSource[manageSourceIndex].Play();
            audioSource[manageSourceIndex].loop = true;
        }
    }

    public void SetMusicAndPlay(NameBGM introBGM, NameBGM loopBGM)
    {
        audioSource[manageSourceIndex].Stop();
        audioSource[manageSourceIndex].time = 0;
        audioSource[manageSourceIndex].clip = audioContainer.GetBGM(introBGM);
        audioSource[manageSourceIndex].Play();
        audioSource[manageSourceIndex].loop = false;
        loopFlag = true;
        nameLoopBGM = loopBGM;
        startFlag = true;
    }
    
    public void SetMusicAndPlay(NameBGM nameBGM, bool isLoop)
    {
        audioSource[manageSourceIndex].Stop();
        audioSource[manageSourceIndex].time = 0;
        audioSource[manageSourceIndex].clip = audioContainer.GetBGM(nameBGM);
        audioSource[manageSourceIndex].Play();
        loopFlag = isLoop;
        audioSource[manageSourceIndex].loop = isLoop;
        startFlag = true;
    }

    public void ChangeMusicSeamless(NameBGM loopBGM, float time)
    {
        int tmp = manageSourceIndex;
        manageSourceIndex = unmanageSourceIndex;
        unmanageSourceIndex = tmp;
        audioSource[manageSourceIndex].clip = audioContainer.GetBGM(loopBGM);
        audioSource[manageSourceIndex].time = audioSource[unmanageSourceIndex].time;
        audioSource[manageSourceIndex].Play();
        audioSource[manageSourceIndex].loop = true;
        StartCoroutine(Fade(time));
    }

    private IEnumerator Fade(float time)
    {
        float passTime = 0;
        while (true)
        {
            passTime += Time.deltaTime;
            audioSource[manageSourceIndex].volume = passTime / time;
            audioSource[unmanageSourceIndex].volume = 1 - passTime / time;
            if (passTime > time)
            {
                break;
            }
            yield return null;
        }
    }

    public void Stop()
    {
        audioSource[manageSourceIndex].Stop();
        audioSource[unmanageSourceIndex].Stop();
        startFlag = false;
    }

    public void PlayOneShot(AudioClip audioClip)
    {
        audioSourceForSE.PlayOneShot(audioClip);
    }
}
