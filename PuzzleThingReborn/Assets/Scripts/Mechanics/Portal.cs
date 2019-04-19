using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Portal : MonoBehaviour
{

    public Transform receiver;
    public Image img;

    public float speed = 1.0f;
    public float activateSpeed = 1.0f;

    Vector3 posisitionOffset;

    Vector3 rotationOffset;

    Color transparent;

    GameObject player;

    bool activated = false;

    public AudioSource audio;

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        transparent = new Color(255.0f, 255.0f, 255.0f, 0.0f);

        posisitionOffset = new Vector3(0.0f, 1.5f, -2.9f);
        rotationOffset = receiver.transform.rotation.eulerAngles;

        
        GetComponent<Renderer>().material.color = Color.white;

        audio = GetComponent<AudioSource>();


    }

    // Teleport the player
    IEnumerator Teleport(GameObject temp)
    {
        float t = 0.0f;

        audio.Play();

        // Screen flash to white
        while (t < 1.1f)
        {
            img.color = Color.Lerp(transparent, Color.white, t);
            t += speed * Time.deltaTime;

            yield return null;
        }


        // Moe and rotate the player
        player.transform.position = receiver.position;// - posisitionOffset;
        //player.transform.rotation = Quaternion.Euler(0.0f, player.transform.rotation.eulerAngles.y - 180.0f, 0.0f);

        yield return new WaitForSeconds(0.2f);

        // Flash the screen to transparent
        t = 0.0f;
        while (t < 1.5f)
        {
            img.color = Color.Lerp(Color.white, transparent, t);
            t += speed * Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        activated = false;

        yield return null;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            if (!activated)
            {
                activated = true;
                StartCoroutine(Teleport(col.gameObject));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}