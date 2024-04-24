using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//////////////////////////////////////////////
//Assignment/Lab/Project: Animations
//Name: Hunter Wright
//Section: SGD.213.2172
//Instructor: Brian Sowers
//Date: 4/15/2024
/////////////////////////////////////////////
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 1000f;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float standHeight = 1.75718f;
    [SerializeField] private float crouchHeight = 1f;

    private CharacterController characterController;
    private float cameraVert = 0f;
    private Animator animator;
    private bool isGrounded;
    private float gravity = -9.8f;
    private float vertVelocity = 0;
    private bool isCrouched = false;


    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;

        cameraVert = -10f;
        cameraTransform.localEulerAngles = new Vector3(cameraVert, 0f, 0f);

        //isGrounded = true;
    }

    // Update is called once per frame
    void Update()
    {
        
        RotateCamera();
        CheckDance();
        UpdateMovement();
        CheckCrouch();

    }

    private void FixedUpdate()
    {
    }

    private void UpdateMovement()
    {
        Vector3 movement = new Vector3();
        movement.x = Input.GetAxis("Horizontal");
        movement.z = Input.GetAxis("Vertical");
        if (!isCrouched)
        {
            CheckSprint();
        }
        float vertSpeed = Input.GetAxis("Vertical") * moveSpeed;
        float horSpeed = Input.GetAxis("Horizontal") * moveSpeed;

        bool isWalking = movement.magnitude > 0f;
        //animator.SetBool("isWalking", isWalking);
        if (!isCrouched)
        {
            animator.SetBool("isWalking", isWalking);
        }
        else if (isCrouched)
        {
            animator.SetBool("isCrouchWalking", isWalking);
        }

        if (characterController.isGrounded && !isCrouched)
        {
            if (Input.GetButtonDown("Jump"))
            {
                animator.SetTrigger("Jump");
            }
            else
            {
                vertVelocity = -2f;
            }
        }
        else
        {
            vertVelocity += gravity * Time.deltaTime;
        }

        Vector3 speed = new Vector3(horSpeed, vertVelocity, vertSpeed);
        speed = transform.rotation * speed;


        characterController.Move(speed * Time.deltaTime);
    }

    private void RotateCamera()
    {
        Vector3 cameraRot = new Vector3();
        cameraRot.x = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        cameraRot.y = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * cameraRot.x);

        cameraVert -= cameraRot.y;
        cameraVert = Mathf.Clamp(cameraVert, -50f, 50f);

        cameraTransform.localEulerAngles = new Vector3(cameraVert, cameraTransform.localEulerAngles.y, 0f);
    }

    private void CheckSprint()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            animator.SetBool("isSprinting", true);
            moveSpeed = moveSpeed * 2;
        } else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            animator.SetBool("isSprinting", false);
            moveSpeed = moveSpeed / 2;
        }
    }

    private void CheckDance()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.F))
        {
            animator.SetTrigger("Dance");
        }
    }

    private void CheckCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouched = !isCrouched;
            if (isCrouched)
            {
                //characterController.height = crouchHeight;
                animator.SetTrigger("toggleCrouch");
            }
            if (!isCrouched)
            {
                animator.SetTrigger("toggleCrouch");
            }
        }
    }
    private void ChangeHeight()
    {
        if (isCrouched)
        {
            characterController.height = crouchHeight;
        }
        if (!isCrouched)
        {
            characterController.height = standHeight;

        }
    }

    private void JumpTrigger()
    {
        vertVelocity = Mathf.Sqrt(2 * -gravity * jumpHeight);
        Vector3 speed = new Vector3(transform.position.x, vertVelocity, transform.position.z);
        speed = transform.rotation * speed;


        characterController.Move(speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            Debug.Log("Touch Grass");
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            Debug.Log("Jumping");
            isGrounded = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Deathplane"))
        {
            transform.position = new Vector3(0, 0.26f, 0);
        }
    }
}
