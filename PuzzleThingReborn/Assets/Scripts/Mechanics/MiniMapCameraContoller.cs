using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCameraContoller : MonoBehaviour
{

    public Light light;

    void OnPreRender()
    {
        
        //light.enabled = true;
        
    }

    void OnPostRender()
    {
        
        //light.enabled = false;
        
    }
}
