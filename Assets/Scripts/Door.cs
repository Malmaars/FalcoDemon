using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Vector2 closedLocation, openLocation;
    public Transform actualDoor;
    public float openAndCloseSpeed;

    bool open;

    private void Update()
    {
        if (open)
        {
            actualDoor.localPosition = Vector3.Lerp(actualDoor.localPosition, openLocation, openAndCloseSpeed * Time.deltaTime);
        }

        else
        {
            actualDoor.localPosition = Vector3.Lerp(actualDoor.localPosition, closedLocation, openAndCloseSpeed * Time.deltaTime);
        }
    }

    public void ChangeDoorState()
    {
        if (open)
            CloseDoor();
        else
            OpenDoor();
    }

    public void OpenDoor()
    {
        //move to the closedLocation

        open = true;
    }

    public void CloseDoor()
    {
        open = false;
    }
}
