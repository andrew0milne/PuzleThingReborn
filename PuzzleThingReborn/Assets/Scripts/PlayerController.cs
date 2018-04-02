using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour 
{
	public float movementSpeed = 2f;
	public float mouseSensitivity = 5f;
	public float upDownRange = 60f;
	public bool cursorLocked = false;

	float forwardSpeed = 0f;
	float sideSpeed = 0f;
	float verticalRotation = 0f;
	float verticalVelocity = 0f;

	CharacterController characterController;

	public Canvas canvas;
    public Text HUDText;

	// Use this for initialization
	void Start () 
	{
		canvas.gameObject.SetActive (true);

		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;

		characterController = GetComponent<CharacterController> ();
	}

	// Update is called once per frame
	void Update () 
	{
		if (cursorLocked == true) 
		{

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(new Vector3((Screen.width / 2), (Screen.height / 2)));

            if (Physics.Raycast(ray.origin, ray.direction, out hit, 5.0f))
            {
                if (hit.collider.gameObject.tag == "MovingObject")
                {
                    HUDText.text = "LMB to press";
                    if (Input.GetMouseButtonDown(0))
                    {
                        hit.collider.gameObject.SendMessage("Activate");
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
            // Camera Rotation
            float rotLeftRight = Input.GetAxis ("Mouse X") * mouseSensitivity;
			transform.Rotate (0, rotLeftRight, 0);

			verticalRotation -= Input.GetAxis ("Mouse Y") * mouseSensitivity;
			verticalRotation = Mathf.Clamp (verticalRotation, -upDownRange, upDownRange);



			// Move the camera
			Camera.main.transform.localRotation = Quaternion.Euler (verticalRotation, 0, 0);

			// Movement
			forwardSpeed = Input.GetAxis ("Vertical") * movementSpeed;
			sideSpeed = Input.GetAxis ("Horizontal") * movementSpeed;

			// Stop the player falling if they're grounded
			verticalVelocity = characterController.isGrounded ? 0 : verticalVelocity + (Physics.gravity.y * Time.deltaTime);

			Vector3 speed = new Vector3 (sideSpeed, verticalVelocity, forwardSpeed);

			speed = transform.rotation * speed;

			characterController.Move (speed * Time.deltaTime);
		}

		//platform_move_offset = Vector3.zero;

		// Pause the game
		if (Input.GetKeyDown (KeyCode.Escape)) 
		{
			TogglePause ();
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

