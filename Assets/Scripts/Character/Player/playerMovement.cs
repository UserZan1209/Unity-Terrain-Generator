using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class playerMovement : MonoBehaviour
{

    [HideInInspector] private float xAxisInput;
    [HideInInspector] private float zAxisInput;

    [SerializeField] PlayerInput InputMap_Def;
    [SerializeField] InputAction walk_def;
    [SerializeField] InputAction jump_def;

    [SerializeField] private CharacterController controller;
    [SerializeField] private Vector3 playerVelocity;
    [SerializeField] private bool groundedPlayer;
    [SerializeField] private float playerSpeed = 3.5f;
    [SerializeField] private float jumpHeight = 3.5f;
    [SerializeField] private float gravityValue = -9.81f;

    [SerializeField] private Transform camTransform;

    private void Start()
    {
        camTransform = Camera.main.transform;

        controller = GetComponent<CharacterController>();
        InputMap_Def = GetComponent<PlayerInput>();

        walk_def = InputMap_Def.actions["walk"];
        jump_def = InputMap_Def.actions["jump"];

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;


    }

    void Update()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector2 input = walk_def.ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0, input.y);
        move = move.x * camTransform.right.normalized + move.z * camTransform.forward.normalized;
        move.y = 0f;
        controller.Move(move * Time.deltaTime * playerSpeed);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }

        // Changes the height position of the player..
        if (jump_def.triggered && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);


        if (Input.GetKeyUp(KeyCode.F1))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
