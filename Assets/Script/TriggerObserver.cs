using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerObserver : MonoBehaviour
{
    public bool Enter { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Enter = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Enter = false;
        }
    }
}
