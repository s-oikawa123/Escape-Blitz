using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Container;

[CreateAssetMenu(menuName = "Create Audio Container")]
public class AudioContainer : ScriptableObject
{
    [SerializeField] private AudioClip[] BGM;
    [SerializeField] private AudioClip[] SE;

    public AudioClip GetBGM(NameBGM name)
    {
        return BGM[(int)name];
    }

    public AudioClip GetAudioClip(NameSE name)
    {
        return SE[(int)name];
    }
}

namespace Container
{
    public enum NameBGM
    {
        NoLightsBehindIntro,
        NoLightsBehindLoop,
        ARoomOfVoid
    }

    public enum NameSE
    {
        DoorOpen,
        LightOut,
        KeyPad,
        KeyPadFalse,
        AllLightOut,
        GasEmission,
        Fall
    }
}
