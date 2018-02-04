using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {
    Room.DoorDir doorDir;
    public void Init(Room.DoorDir _doorDir,Vector3 _position)
    {
        doorDir = _doorDir;
        transform.position = _position;
        switch (doorDir)
        {
            case Room.DoorDir.DOWN:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                this.GetComponent<Collider2D>().offset = new Vector3(-0.5f, -0.5f);
                break;
            case Room.DoorDir.UP:
                transform.rotation = Quaternion.Euler(0, 0, 180);
                this.GetComponent<Collider2D>().offset = new Vector3(0.5f, 0.5f);
                break;
            case Room.DoorDir.LEFT:
                transform.rotation = Quaternion.Euler(0, 0, 90);
                this.GetComponent<Collider2D>().offset = new Vector3(-0.5f, 0.5f);
                break;
            case Room.DoorDir.RIGHT:
                transform.rotation = Quaternion.Euler(0, 0, -90);
                this.GetComponent<Collider2D>().offset = new Vector3(0.5f, -0.5f);
                break;
        }
    }
}
