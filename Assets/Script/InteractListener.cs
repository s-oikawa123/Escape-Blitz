using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

// �C���^���N�g���X�i�[�iRayCast���󂯂�obj����eobj��InteractableObjectManager���擾������������N���X�j
public class InteractListener : MonoBehaviour
{
    [SerializeField] private InteractableObjectManager interactableObjectManager;
    public InteractableObjectManager InteractableObjectManager { get { return interactableObjectManager; } }
}
