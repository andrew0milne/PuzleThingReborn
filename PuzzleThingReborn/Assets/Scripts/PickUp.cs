using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    Vector3 start_pos;

    Vector3 start_scale;

    Vector2 perlin_pos;

    public float speed = 1.0f;
    public float perlin_scale = 0.2f;

    float scale_factor;

    Vector3 pos;
    Vector3 sca;

    Light light;

    public AudioSource audio;

    private void Start()
    {
        start_pos = transform.position;
        start_scale = transform.localScale;

        scale_factor = start_scale.x;

        perlin_pos.x = start_pos.x * perlin_scale;
        perlin_pos.y = start_pos.z * perlin_scale;

        light = GetComponent<Light>();

        audio = GetComponent<AudioSource>();
    }

    IEnumerator Kill()
    {
        audio.pitch = Random.Range(0.8f, 1.2f);

        audio.Play();

        GetComponent<Renderer>().enabled = false;
        GetComponent<SphereCollider>().enabled = false;
        GetComponent<Light>().enabled = false;

        while(audio.isPlaying)
        {
            yield return null;
        }

        Destroy(this.gameObject);

        yield return null;
    }

    public void Activate()
    {
        StartCoroutine(Kill());
    }

    // Update is called once per frame
    void Update ()
    {
        float new_y = Mathf.PerlinNoise(perlin_pos.x + Time.time * speed, perlin_pos.y + Time.time * speed);

        light.intensity = new_y + 0.5f;


        pos = start_pos;
        sca = start_scale;

        pos.y = (start_pos.y / 2.0f) + new_y;

        float new_sca = start_scale.x / 2.0f + new_y * scale_factor;
        sca = new Vector3(new_sca, new_sca, new_sca);

        transform.position = pos;
        transform.localScale = sca;
    }
}
