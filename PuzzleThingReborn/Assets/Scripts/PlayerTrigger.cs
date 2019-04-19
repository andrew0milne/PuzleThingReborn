using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{

    // Use this for initialization
    public List<GameObject> targets;

    void Activate()
    {
        foreach (GameObject g in targets)
        {
            g.SendMessage("Activate");
        }
    }

    void Deactivate()
    {

    }

}
