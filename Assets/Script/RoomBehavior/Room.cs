using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Container;

public class Room : RoomInfo
{
    [SerializeField] private GameObject[] exitports;
    [SerializeField] private Transform goalObserverPivot;
    private List<DoorManager> doorManagers = new List<DoorManager>();

    // ルームプロパティ
    private int doorCount;
    private int spreadMax;
    private int spreadMin;
    private protected override void SetUp()
    {
        if (wrongRoomFrag)
        {
            summery = GameManager.Text_C.FailReason.RoomRoom[1].GetString();
            exitTime = 3.7f;
        }
        else
        {
            summery = GameManager.Text_C.FailReason.RoomRoom[0].GetString();
            exitTime = 3.4f;
        }
        GenrateDoor();
    }

    private protected override void First()
    {
        
    }

    private protected override void Every()
    {

    }

    public override void Signal(Signal signal) 
    { 
        if (signal == Container.Signal.True)
        {
            gameManager.ActiveNextRoom();
            roomClearedFrag = true;
            foreach (var manager in doorManagers)
            {
                manager.SetActive(false);
            }
            backDoorManager.SetActive(false);
        }

        if (signal == Container.Signal.False)
        {
            gameManager.GameOverInitiation(summery);
        }
    }

    private protected override void ForceParam(int number)
    {
        switch (number)
        {
            case 1:
                doorCount = 1;
                wrongRoomFrag = false;
                break;
            case 2:
                doorCount = 3;
                spreadMax = 5;
                spreadMin = 3;
                wrongRoomFrag = false;
                break;
            case 8:
                doorCount = 1;
                wrongRoomFrag = true;
                break;
            case 9:
                doorCount = 1;
                wrongRoomFrag = false;
                break;
        }
    }

    [ParameterDecision(50, 0)]
    public void DoorCount(float intensity)
    {
        doorCount = 1;
        for (int i = 1; i <= 5; i++)
        {
            if (intensity <= i * 10)
            {
                doorCount = i;
                break;
            }
        }
    }

    [ParameterDecision(50, 0)]
    public void DoorNumber(float intensity)
    {
        float maxSpreadIntensity = intensity * Random.Range(0f, 0.5f);
        float minSpreadIntensity = intensity - maxSpreadIntensity;
        if (minSpreadIntensity > 25)
        {
            maxSpreadIntensity += minSpreadIntensity - 25;
            minSpreadIntensity = 25;
        }

        spreadMax = 6;
        for (int i = 1; i <= 5; i++)
        {
            if (maxSpreadIntensity <= i * 5)
            {
                spreadMax -= i;
                break;
            }
        }

        spreadMin = 6;
        for (int i = 1; i <= 5; i++)
        {
            if (minSpreadIntensity <= i * 5)
            {
                spreadMin -= i;
                break;
            }
        }
    }


    private void GenrateDoor()
    {
        List<int> availablePort = Enumerable.Range(0, 5).ToList();
        List<int> diffNumber = new List<int>();
        diffNumber.Add(0);
        for (int i = 0; i < doorCount - 1; i++)
        {
            diffNumber.Add(diffNumber[^1] + Random.Range(spreadMin, spreadMax + 1));
        }
        int trueDoorIndex = Random.Range(0, doorCount);
        int diffOffset = 0;
        int trueDoorDiff = diffNumber[trueDoorIndex];

        if (wrongRoomFrag)
        {
            List<int> search = Enumerable.Range(-3, diffNumber[^1] + 7).Except(diffNumber).Select(a => trueDoorDiff - a).ToList();
            int index = Random.Range(0, search.Count);
            diffOffset = search[index];
        }

        diffNumber = diffNumber.Select(a => a + diffOffset).ToList();


        for (int i = 0; i < doorCount; i++)
        {
            GameObject go = Instantiate(roomContainer.GetRoomProp(RoomProp.Door), transform);
            DoorManager doorManager = go.GetComponent<DoorManager>();
            doorManagers.Add(doorManager);
            doorManager.SetInteractToOpen(true);
            InteractableObjectManager manager = go.GetComponent<InteractableObjectManager>();
            int portIndex = Random.Range(0, availablePort.Count);
            go.transform.localPosition = new Vector3(-4 + 2 * availablePort[portIndex], 1.5f, 20);
            doorManager.SetRoomNumber(RoomNumber + diffNumber[i] - trueDoorDiff);

            if (i == trueDoorIndex)
            {
                manager.SetSignal(Container.Signal.True);
                manager.SetActive(true);
                manager.SetRoomInfo(this);
                exitPoint = new Vector3(-4 + 2 * availablePort[portIndex], 0, 20);
                goalObserverPivot.localPosition = new Vector3(-4 + 2 * availablePort[portIndex], 2, 21);
                this.doorManager = doorManager;
            }
            else
            {
                manager.SetSignal(Container.Signal.False);
                manager.SetActive(true);
                manager.SetRoomInfo(this);
                doorManager.SetFalseDoor(true);
            }

            availablePort.RemoveAt(portIndex);
        }

        foreach (var i in availablePort)
        {
            exitports[i].transform.localScale = new Vector3(1, 4, 1);
        }
    }
}
