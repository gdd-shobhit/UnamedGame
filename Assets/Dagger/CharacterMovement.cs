using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float runSpeed = 8f;
    public float crouchedSpeed = 2f;
    public float mouseSensitivity = 2f;
    public float jumpHeight = 5;
    public GameObject cameraPivot;

    public float playerSpeed;
    private float cameraRotateX = 0f;
    private bool isGrounded = false;
    private Rigidbody rigidBody;
    private Animator animator;
    private CapsuleCollider capsuleCollider;
    private float capsuleHalfHeight;

    private DaggerInput inputActions;
    private Vector2 movementInput;
    private Vector2 cameraInput;
    private bool canJump;
    void Start()
    {
        //--get components--
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        capsuleHalfHeight = capsuleCollider.height / 2;

        //--hide the mosue cursor. Press Esc during play to show the cursor. --
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        inputActions = GetComponent<DaggerInput>();
    }

    public void OnEnable()
    {
        if(inputActions == null)
        {
            inputActions = new DaggerInput();
            inputActions.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            inputActions.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
            inputActions.PlayerMovement.canJump.performed += i => canJump = true;
            inputActions.PlayerMovement.cantJump.performed += i => canJump = false;
        }

        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    void Update()
    {
        //--get values used for character and camera movement--
        float horizontalInput = movementInput.x;//Input.GetAxis("Horizontal");
        float verticalInput = (movementInput.y);//Input.GetAxis("Vertical");
        float mouse_X = cameraInput.x;//Input.GetAxis("Mouse X")*mouseSensitivity;
        float mouse_Y = -1 * cameraInput.y;//Input.GetAxis("Mouse Y")*mouseSensitivity;
        //normalize horizontal and vertical input (I am not sure about this one but it seems to work :P)
        float normalizedSpeed = Vector3.Dot(new Vector3(horizontalInput, 0f, verticalInput).normalized, new Vector3(horizontalInput, 0f, verticalInput).normalized);

        //--camera movement and character sideways rotation--
        transform.Rotate(0, mouse_X, 0);
        cameraRotateX += mouse_Y;
        cameraRotateX = Mathf.Clamp(cameraRotateX, -15, 60); //limites the up/down rotation of the camera 
        cameraPivot.transform.localRotation = Quaternion.Euler(cameraRotateX, 0, 0);

        //--check if character is on the ground
        CheckGround();

        //--sets Speed, "inAir" and "isCrouched" parameters in the Animator--
        animator.SetFloat("speed", playerSpeed);
        animator.SetBool("isInAir", canJump);

        //--change playerSpeed and Animator Parameters when the "run" or "crouch" buttons are pressed--
        //if (playerSpeed > 0/*Input.GetButton("Run")*/)
        //{
        //    transform.Translate(new Vector3(horizontalInput, 0, verticalInput) * runSpeed * Time.deltaTime);
        //    playerSpeed = Mathf.Lerp(playerSpeed, normalizedSpeed * runSpeed, 0.05f);
        //}
        //else if (Input.GetButton("Crouch"))
        //{
        //    isCrouched = true;
        //    transform.Translate(new Vector3(horizontalInput, 0, verticalInput) * crouchedSpeed * Time.deltaTime);
        //    playerSpeed = Mathf.Lerp(playerSpeed, normalizedSpeed * crouchedSpeed, 0.05f);
        //    animator.SetBool("isCrouched", true);
        //}
        //else //this is the standard walk behaviour 
        //{
            transform.Translate(new Vector3(horizontalInput, 0, verticalInput) * walkSpeed * Time.deltaTime);
            playerSpeed = Mathf.Lerp(playerSpeed, normalizedSpeed * walkSpeed, 1f);
        //}


        ////--Jump behaviour--
        if (canJump && isGrounded)
        {
            rigidBody.velocity = new Vector3(0, jumpHeight, 0);
        }
        if (!isGrounded)
        {
            animator.SetBool("inInAir", true);
        }

        ////--Play the "Special" animation --
        //if (Input.GetButtonDown("Special"))
        //{
        //    animator.SetTrigger("Special");
        //}
    }

    void CheckGround()
    {
        //--send a ray from the center of the collider to the ground. The player is "grounded" if the ray distance(length) is equal to half of the capsule height--
        Physics.Raycast(capsuleCollider.bounds.center, Vector3.down, out var hit);
        if (hit.distance < (capsuleHalfHeight + 0.1f))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

}
