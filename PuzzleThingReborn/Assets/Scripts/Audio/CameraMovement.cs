using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public MidiReader reader;

    float speed = 0.0f;

    public float s;

    // Use this for initialization
    void Start()
    {

    }

    public IEnumerator SetUp()
    {
        speed = reader.speed;

        Debug.Log(speed + "  hello");

        yield return null;
    }

    void Activate()
    {
        StartCoroutine(SetUp());
    }

	// Update is called once per frame
	void Update ()
    {
        transform.position += new Vector3(s * Time.deltaTime, 0.0f, 0.0f);
	}
}
