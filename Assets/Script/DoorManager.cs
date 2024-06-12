using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Container;

public class DoorManager : InteractableObjectManager
{
    [SerializeField] private TMP_Text numberText;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioContainer audioContainer;
    [SerializeField] private bool falseDoor;
    [SerializeField] private bool interactToOpen;
    [SerializeField] private ParticleSystem particle;
    public bool FalseDoor { get { return falseDoor; } }

    public void SetFalseDoor(bool value)
    {
        falseDoor = value;
    }

    public void SetInteractToOpen(bool value)
    {
        interactToOpen = value;
    }

    public void SetRoomNumber(int number)
    {
        numberText.text = number.ToString();
    }

    public void HideRoomNumber()
    {
        numberText.gameObject.SetActive(false);
    }

    public void Open()
    {
        animator.SetTrigger("Open");
        audioSource.PlayOneShot(audioContainer.GetAudioClip(NameSE.DoorOpen));

        if (falseDoor)
        {
            audioSource.PlayOneShot(audioContainer.GetAudioClip(NameSE.GasEmission));
            particle.Play();
        }
    }

    public override void Interact()
    {
        roomInfo.Signal(signal);
        if (interactToOpen)
        {
            Open();
        }
    }
}
