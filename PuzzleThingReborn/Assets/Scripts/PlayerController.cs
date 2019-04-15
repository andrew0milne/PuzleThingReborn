using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour 
{
    Rigidbody rb;
    public int score = 0;
    public Text score_text;

    //public GameObject contact_point;

    [SerializeField]
    float movement_speed = 5.0f, jump_force; //grav_change_speed = 5.0f,
    float forward_speed, side_speed, vertical_speed;
    Vector3 speed;

    [SerializeField]
    float mouse_sensitivity = 60.0f, vertical_rotation = 0.0f, up_down_range = 60.0f;

    CharacterController char_contr;

    Vector3 gravity_dir;

    bool grounded;
    bool reorienting = false;

    List<GameObject> objects_colliding_with;

   
	public bool cursorLocked = false;

	
	public Canvas canvas;
    public Text HUDText;

	public GameObject beam_gun;

    Vector3 direction = Vector3.zero;
    Vector3 old_pos = Vector3.zero;

    bool grav_gun = true;

    public GameObject game_controller;

    // Use this for initialization
    void Start () 
	{
		canvas.gameObject.SetActive (true);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        grounded = false;

        objects_colliding_with = new List<GameObject>();

        rb = GetComponent<Rigidbody>();
        char_contr = GetComponent<CharacterController>();

        gravity_dir = new Vector3(0.0f, -1.0f, 0.0f);

        Physics.IgnoreLayerCollision(9, 10);
    }

    void UserInput()
    {
        
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3((Screen.width / 2), (Screen.height / 2)));

        if (Physics.Raycast(ray.origin, ray.direction, out hit, 5.0f))
        {
            if (hit.collider.gameObject.tag == "MovingObject")
            {
                if (hit.collider.gameObject.GetComponent<MovingObject>().clickable == true)
                {
                    HUDText.text = "LMB to press";
                    if (Input.GetMouseButtonDown(0))
                    {
                        hit.collider.gameObject.SendMessage("Activate");

                    }
                }

            }
            else if (hit.collider.gameObject.tag == "PuzzleReset")
            {
                HUDText.text = "LMB reset the Puzzle";
                if (Input.GetMouseButtonDown(0))
                {
                    hit.collider.gameObject.SendMessage("Reset");
                }

            }
            else
            {
                HUDText.text = "";
            }

        }
        else
        {
            HUDText.text = "";

        }

        if (Input.GetMouseButtonDown(1))
        {
            beam_gun.SendMessage("Activate");
        }
        else if (Input.GetMouseButtonUp(1))
        {
            beam_gun.SendMessage("Deactivate");
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            grav_gun = !grav_gun;
        }

        // Camera Rotation
        float rotLeftRight = Input.GetAxis("Mouse X") * mouse_sensitivity;
        transform.Rotate(0, rotLeftRight, 0);

        vertical_rotation -= Input.GetAxis("Mouse Y") * mouse_sensitivity;
        vertical_rotation = Mathf.Clamp(vertical_rotation, -up_down_range, up_down_range);

        // Move the camera
        Camera.main.transform.localRotation = Quaternion.Euler(vertical_rotation, 0, 0);


        Move();
        
    }

    void Move()
    {
        if (grounded)
        {

            // Movement
            forward_speed = Input.GetAxis("Vertical") * movement_speed * Time.deltaTime;
            side_speed = Input.GetAxis("Horizontal") * movement_speed * Time.deltaTime;

            

            if (Input.GetButtonDown("Jump"))
            {
                rb.AddForce(gravity_dir * -1 * jump_force);
                //rb.AddForce(direction.normalized * jump_force);
            }            
        }


        speed = new Vector3(side_speed, 0.0f, forward_speed);
        transform.Translate(speed);

        direction = transform.position - old_pos;

        //speed = new Vector3(side_speed, 0.0f, forward_speed);
        //transform.Translate(speed);

        old_pos = transform.position;

        rb.AddForce(gravity_dir * 9.8f);
    }

    // Update is called once per frame
    void Update () 
	{
		if (cursorLocked == true) 
		{


            UserInput();
            //UpdateContactPoint();

            if (objects_colliding_with.Count == 0)
            {
                grounded = false;
            }
            else
            {
                grounded = true;
            }
        }

		//platform_move_offset = Vector3.zero;

		// Pause the game
		if (Input.GetKeyDown (KeyCode.Escape)) 
		{
			TogglePause ();
		}


	}

    private void OnTriggerEnter(Collider col)
    {
        switch(col.tag)
        {
            case "PickUp":
                {
                    score++;
                    score_text.text = score.ToString();
                    Destroy(col.gameObject);
                    GameController.instance.UpdateScore(score);
                    break;
                }
            case "Activator":
                {
                    col.gameObject.SendMessage("Activate");                 
                    break;
                }
            default:
                break;

        }
    }

    private void OnTriggerExit(Collider col)
    {
        switch (col.tag)
        {
            
            case "Activator":
                {
                    col.gameObject.SendMessage("Deactivate");
                    break;
                }
            default:
                break;

        }
    }

    //void UpdateContactPoint()
    //{
    //    if (grav_gun)
    //    {
    //        RaycastHit hit;
    //        Ray ray = Camera.main.ScreenPointToRay(new Vector3((Screen.width / 2), (Screen.height / 2)));

    //        if (Physics.Raycast(ray.origin, ray.direction, out hit, 50.0f))
    //        {
    //            if (hit.transform.tag != "Player")
    //            {
    //                contact_point.SetActive(true);
    //                contact_point.transform.position = hit.point;

    //                if (Input.GetMouseButtonDown(0))
    //                {
    //                    gravity_dir = -1 * hit.normal;

    //                }
    //                contact_point.transform.up = hit.normal;
    //            }
    //        }
    //        else
    //        {
    //            contact_point.SetActive(false);
    //        }
    //    }
    //    else
    //    {
    //        contact_point.SetActive(false);
    //    }
    //}

    //IEnumerator Reorient(Vector3 new_up)
    //{
    //    reorienting = true;
    //    float time = 0.0f;

    //    Vector3 old_up = transform.up;

    //    Vector3 rot = transform.rotation.eulerAngles;
    //    Vector3 old_rot = transform.rotation.eulerAngles;

    //    if (new_up == transform.up * -1)
    //    {         
    //        float angle_count = 0.0f;
    //        while (angle_count < 180.0f)
    //        {
    //            float num = Time.deltaTime * grav_change_speed * 500.0f;

    //            rot.z += num;
    //            transform.rotation = Quaternion.Euler(rot);
    //            angle_count += num;

    //            yield return null;
    //        }

    //        old_rot.z += 180.0f;
    //        transform.rotation = Quaternion.Euler(old_rot);
    //    }
    //    else
    //    {
    //        while (time < 1.1f)
    //        {
    //            Vector3 forward = Vector3.Cross(transform.right, new_up);
    //            Quaternion target_rot = Quaternion.LookRotation(forward, new_up);
    //            transform.rotation = Quaternion.Lerp(transform.rotation, target_rot, time);

    //            //transform.up = Vector3.Slerp(old_up, new_up, time);
    //            //Camera.main.transform.localRotation.SetLookRotation(hit.point);

    //            time += Time.deltaTime * grav_change_speed;
    //            yield return null;
    //        }
    //    }


    //    reorienting = false;
    //    yield return null;
    //}

    void OnCollisionEnter(Collision col)
    {
        //if (col.gameObject.layer == 11)
        //{
            objects_colliding_with.Add(col.gameObject);

            //if (transform.up != gravity_dir * -1 && !reorienting)
            //{
            //    StartCoroutine(Reorient(gravity_dir * -1));

            //}
        //}
    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.layer == 11)
        {
            objects_colliding_with.Remove(col.gameObject);
        }
    }

    void TogglePause()
	{
		if (cursorLocked == true) 
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			cursorLocked = false;
		} 
		else
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			cursorLocked = true;
		}
	}
}

