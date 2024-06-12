using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Container;

// インタラクト可能なobjの基礎機能
public abstract class InteractableObjectManager : MonoBehaviour
{
    [SerializeField] private protected Signal signal;
    [SerializeField] private protected RoomInfo roomInfo;
    [SerializeField] private protected bool isActive;
    [SerializeField] private protected Transform interactIconTrans;
    public bool IsActive { get { return isActive; } }
    public Signal Signal { get { return signal; } }
    public Vector3 InteractIconPos { get { return interactIconTrans.position; } }

    public void SetRoomInfo(RoomInfo roomInfo)
    {
        this.roomInfo = roomInfo;
    }

    public void SetSignal(Signal signal)
    {
        this.signal = signal;
    }

    public void SetActive(bool isActive)
    {
        this.isActive = isActive;
    }

    public abstract void Interact();
}
