using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class pp_controller : MonoBehaviour
{

    public PostProcessingProfile ppp;

    ChromaticAberrationModel.Settings ca;

    public float num = 10.0f;

	// Use this for initialization
	void Start ()
    {
        ca = ppp.chromaticAberration.settings;
	}
	
	// Update is called once per frame
	void Update ()
    {
        ca.intensity = num;

        ppp.chromaticAberration.settings = ca;
	}
}
