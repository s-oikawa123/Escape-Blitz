using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Container;

[CreateAssetMenu(menuName = "Create RoomAsset")]
public class RoomContainer : ScriptableObject
{
    [SerializeField] private GameObject[] rooms;
    [SerializeField] private GameObject[] roomProps;

    public int GetRoomCount()
    {
        return rooms.Length;
    }

    public GameObject GetRoom(int roomId)
    {
        return rooms[roomId];
    }

    public GameObject GetRoomProp(RoomProp roomProp)
    {
        return roomProps[(int)roomProp];
    }
}

namespace Container
{
    public enum RoomProp
    {
        Door,
        Mirror,
        Quad
    }
}
