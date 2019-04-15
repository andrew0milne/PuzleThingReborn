using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomRoof : MonoBehaviour
{

    public bool stays_gone = false;

    void Activate()
    {
        if (stays_gone)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.layer = 12;
        }
    }

    void Deactivate()
    {
        gameObject.layer = 0;
    }

}
