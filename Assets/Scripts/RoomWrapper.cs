using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomWrapper : MonoBehaviour
{
    Room room;
    List<Door> doorList;
    int enemyCapacity;
    int roomSize;

    public void Init(Room _room,Vector2 _position,int _enemyCapacity)
    {
        room = _room;
        transform.position = _position;
        enemyCapacity = _enemyCapacity;
        roomSize = enemyCapacity;
    }
    public void SetDoorList()
    {
        doorList = room.CreateDoorObject(this.transform); 
    }
    public Room GetRoom()
    {
        return room;
    }
}
