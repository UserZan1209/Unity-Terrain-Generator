using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class aircraftController : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;

    [SerializeField] PlayerInput InputMap_Def;
    [SerializeField] InputAction Thrust_def;
    [SerializeField] InputAction Turn_def;

    [SerializeField] private CharacterController controller;
    [SerializeField] private Vector3 playerVelocity;
    [SerializeField] private bool groundedPlayer;
    [SerializeField] private float playerSpeed = 3.5f;
    [SerializeField] private float jumpHeight = 3.5f;
    [SerializeField] private float gravityValue = -9.81f;

    [SerializeField] private Transform camTransform;

    [SerializeField] private float engineForce;
    [SerializeField] private float turnSpeed = 500;

    [SerializeField] bool isFlying;

    private void Start()
    {
        camTransform = Camera.main.transform;

        controller = GetComponent<CharacterController>();
        InputMap_Def = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();

        Thrust_def = InputMap_Def.actions["thrust"];
        Turn_def = InputMap_Def.actions["turn"];

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;


    }

    void Update()
    {
/*        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }*/

        Vector2 turn = Turn_def.ReadValue<Vector2>();
 
        //move.y = 0f;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(new Vector3(0,0,engineForce * Time.deltaTime), ForceMode.Impulse);
        }

        //controller.Move(move * Time.deltaTime * playerSpeed);

        if (playerVelocity != Vector3.zero)
        {
/*            playerVelocity.y -= gravityValue;
            isFlying = true;*/
        }
        else
        {
            
        }

        if (isFlying)
        {
            Quaternion targetRotation = new Quaternion(transform.rotation.x,transform.rotation.y * Input.GetAxis("Mouse Y"), transform.rotation.z,1);

            transform.rotation =  Quaternion.Lerp(transform.rotation, targetRotation, turnSpeed);
        }


        // controller.Move(playerVelocity * Time.deltaTime);


        if (Input.GetKeyUp(KeyCode.F1))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
