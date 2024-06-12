using System.Collections;
using UnityEngine;
using Container;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;

// 部屋の基礎クラス
public abstract class RoomInfo : MonoBehaviour
{
    [SerializeField] private protected GameManager gameManager;
    [SerializeField] private protected TriggerObserver goalTriggerObserver;
    [SerializeField] private protected DoorManager doorManager;
    [SerializeField] private protected DoorManager backDoorManager;
    [SerializeField] private protected RoomContainer roomContainer;
    [SerializeField] private protected Vector3 exitPoint;
    [SerializeField] private protected float exitRotation;
    [SerializeField] private protected bool hasAnomalyFlag;
    [SerializeField] private Transform frontGoalPosition;
    [SerializeField] private Transform backGoalPosition;
    [SerializeField] private protected Light[] roomLight;
    private protected float exitTime;
    private protected string summery;
    public Vector3 ExitPoint { get { return exitPoint; } }
    public float ExitRotation { get { return exitRotation; } }
    public float ExitTime { get { return exitTime; } }
    private protected bool roomClearedFrag = false;
    private int roomNumber;
    private protected int RoomNumber { get { return roomNumber; } }
    private protected bool wrongRoomFrag = false;
    private bool backDoorVisibleFrag = false;
    private List<ParameterDecisionContent> parameterDecitions = new List<ParameterDecisionContent>();

    // 基礎的な部屋のセットアップ
    public void SetUpManager(GameManager gameManager)
    {
        this.gameManager = gameManager;
        roomNumber = gameManager.RoomCount;

        doorManager?.SetRoomInfo(this);
        backDoorManager?.SetRoomInfo(this);
        backDoorManager?.gameObject.SetActive(false);
        doorManager?.SetRoomNumber(roomNumber);
        backDoorManager?.HideRoomNumber();

        SetParameterDecision();

        List<float> weights = new List<float>();
        for (int i = 0; i < parameterDecitions.Count; i++)
        {
            weights.Add(UnityEngine.Random.Range(0f, 1f));
        }
        float sum = weights.Sum();

        weights = weights.Select(a => a / sum).ToList();

        for (int i = 0; i < parameterDecitions.Count; i++)
        {
            parameterDecitions[i].SetWeight(weights[i]);
        }

        parameterDecitions = parameterDecitions.OrderByDescending(a => a.Weight).ToList();
        float exceedIntensity = 0;
        float totalWeight = 1;

        foreach (var p in parameterDecitions)
        {
            float intensity = roomNumber * p.Weight + exceedIntensity * p.Weight / totalWeight;
            totalWeight -= p.Weight;
            exceedIntensity += p.CheckIntensity(intensity);
        }

        parameterDecitions = parameterDecitions.OrderByDescending(a => a.Priority).ToList();

        foreach (var p in parameterDecitions)
        {
            p.Invoke();
        }

        ForceParam(gameManager.TutorialNumber);

        SetUp();

        if (wrongRoomFrag)
        {
            SetWrongBehavior();
        }
    }

    // 基礎的なアクティブ中の処理
    public IEnumerator UpdateManager()
    {
        First();
        while (true)
        {
            Every();

            if (roomNumber == 1)
            {
                backDoorManager.gameObject.SetActive(true);
                backDoorVisibleFrag = true;
            }

            float dot = Vector3.Dot(gameManager.Player.transform.position - transform.position, transform.forward);
            if (!backDoorVisibleFrag && backDoorManager != null && dot >= 0)
            {
                Vector3 cameraVector = Camera.main.transform.forward;
                Vector3 backDoorVector = backDoorManager.transform.position - Camera.main.transform.position;
                Vector2 cameraVectorXZ = new Vector2(cameraVector.x, cameraVector.z);
                Vector2 backDoorVectorXZ = new Vector2(backDoorVector.x, backDoorVector.z);

                if (Vector2.Angle(cameraVectorXZ, backDoorVectorXZ) >= Camera.main.fieldOfView && backDoorVector.sqrMagnitude >= 25)
                {
                    gameManager.DeactivatePreviousRoom();
                    backDoorManager.gameObject.SetActive(true);
                    backDoorVisibleFrag = true;
                }
            }


            if (goalTriggerObserver.Enter && roomClearedFrag)
            {
                gameManager.RoomClear();
            }
            yield return null;
        }
    }

    // セットアップ
    private protected abstract void SetUp();
    // アクティブ化最初の処理
    private protected abstract void First();
    // アクティブ中の処理
    private protected abstract void Every();
    // 信号受信
    public abstract void Signal(Signal signal);
    // パラメータの強制決定
    private protected abstract void ForceParam(int number);
    // 消灯アニメーション
    public virtual IEnumerator LightOut()
    {
        float t = 1f;
        float[] intensity = roomLight.Select(a => a.intensity).ToArray();
        while (t > 0)
        {
            t -= Time.deltaTime;
            for (int i = 0; i < intensity.Length; i++)
            {
                roomLight[i].intensity = intensity[i] * t;
            }
            yield return null;
        }
    }

    // 異常のある部屋の基礎的な振る舞いの設定
    private void SetWrongBehavior()
    {
        if (doorManager.Signal == Container.Signal.True)
        {
            doorManager.SetSignal(Container.Signal.False);
            doorManager.SetFalseDoor(true);
        }
        doorManager.SetFalseDoor(true);
        backDoorManager.SetSignal(Container.Signal.True);
        backDoorManager.SetFalseDoor(false);
        goalTriggerObserver.transform.SetParent(backGoalPosition);
        goalTriggerObserver.transform.localRotation = Quaternion.identity;
        goalTriggerObserver.transform.localPosition = Vector3.zero;
        exitPoint = Vector3.zero;
        exitRotation = 180;
    }

    // 異常のある部屋に設定
    public void SetAsWrongRoom()
    {
        if (hasAnomalyFlag)
        {
            wrongRoomFrag = true;
        }
    }

    // 部屋パラメータ決定関数の取得
    private void SetParameterDecision()
    {
        foreach (MethodInfo info in GetType().GetMethods())
        {
            Attribute attribute = Attribute.GetCustomAttribute(info, typeof(ParameterDecisionAttribute));
            if (attribute != null)
            {
                ParameterDecisionAttribute pAtt = attribute as ParameterDecisionAttribute;
                Action<float> action = info.CreateDelegate(typeof(Action<float>), this) as Action<float>;
                ParameterDecisionContent p = new ParameterDecisionContent(action, pAtt.MaxIntensity, pAtt.Priority);
                parameterDecitions.Add(p);
            }
        }
    }
}

// 部屋パラメータ決定関数保存および演算クラス
public class ParameterDecisionContent
{
    private Action<float> action;
    private float maxIntencity;
    private float weight;
    private float intensity;
    private int priority;
    public float Weight { get { return weight; } }
    public int Priority { get { return priority; } }


    public ParameterDecisionContent(Action<float> action, float maxIntencity, int priority)
    {
        this.action = action;
        this.maxIntencity = maxIntencity;
        this.priority = priority;
        this.weight = 0;
        this.intensity = 0;
    }

    public void SetWeight(float weight)
    {
        this.weight = weight;
    }

    public float CheckIntensity(float intensity)
    {
        if (intensity < maxIntencity)
        {
            this.intensity = intensity;
            return 0;
        }
        else
        {
            this.intensity = maxIntencity;
            return intensity - maxIntencity;
        }
    }

    public void Invoke()
    {
        action(intensity);
        Debug.Log($"{action.Method.Name}, Intensity : {intensity}");
    }
}

// 部屋パラメータ決定関数属性
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class ParameterDecisionAttribute : Attribute
{
    private float maxIntensity;
    private int priority;
    public ParameterDecisionAttribute(float maxIntensity, int priority)
    {
        this.maxIntensity = maxIntensity;
        this.priority = priority;
    }

    public float MaxIntensity { get { return maxIntensity; } }
    public int Priority { get { return priority; } }
}

namespace Container
{
    public enum Signal
    {
        True,
        False,
        Challenge,
        Complete,
        Leave
    }
}
