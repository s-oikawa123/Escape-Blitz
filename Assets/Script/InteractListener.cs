using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

// インタラクトリスナー（RayCastを受けるobjから親objのInteractableObjectManagerを取得えお実現するクラス）
public class InteractListener : MonoBehaviour
{
    [SerializeField] private InteractableObjectManager interactableObjectManager;
    public InteractableObjectManager InteractableObjectManager { get { return interactableObjectManager; } }
}
