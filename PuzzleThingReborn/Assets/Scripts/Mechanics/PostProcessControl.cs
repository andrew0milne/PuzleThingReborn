using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class PostProcessControl : MonoBehaviour
{

    public PostProcessingProfile ppp;

    public GameObject player;
    public Transform portal;
    public Transform max_distance_marker;

    public float perlin_scale;
    public float perlin_speed;

    public float abberation_scale;

    ChromaticAberrationModel.Settings aberration_model;

    bool player_here = false;

    float max_distance = 0.0f;

    // Use this for initialization
    void Start()
    {
        aberration_model = ppp.chromaticAberration.settings;

        aberration_model.intensity = 0.0f;
        ppp.chromaticAberration.settings = aberration_model;

        player = GameObject.FindGameObjectWithTag("Player");

        max_distance = Vector3.Distance(portal.position, max_distance_marker.position);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            player_here = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player")
        {
            player_here = false;
            aberration_model.intensity = 0.0f;
            ppp.chromaticAberration.settings = aberration_model;
        }
    }

    float GetDistance()
    {
        float dist = Vector3.Distance(gameObject.transform.position, player.transform.position);

        return (max_distance - dist) / max_distance;
    }

    // Update is called once per frame
    void Update()
    {
        if (player_here)
        {
            aberration_model.intensity = (GetDistance() * abberation_scale) + ((Mathf.PerlinNoise(Time.time * perlin_speed, 0.0f) - 0.5f) * perlin_scale * 2.0f);
            ppp.chromaticAberration.settings = aberration_model;
        }
    }
}
