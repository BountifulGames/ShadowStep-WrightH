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
    [SerializeField] private float speed = 4.0f;  
    [SerializeField] private float mouseSensitivity = 1000f;
    [SerializeField] private Transform cameraTransform;

    private float cameraVert = 0f;
    private Rigidbody rb;
    private Animator animator;
    private bool isGrounded;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();

        cameraVert = -10f;
        cameraTransform.localEulerAngles = new Vector3(cameraVert, 0f, 0f);

        isGrounded = true;
    }

    // Update is called once per frame
    void Update()
    {
        
        RotateCamera();
        CheckSprint();
        CheckDance();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            animator.SetTrigger("Jump");
            Invoke("CheckJump", 0.55f);
        }
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void Movement()
    {
        Vector3 movement = new Vector3();
        movement.x = Input.GetAxis("Horizontal");
        movement.z = Input.GetAxis("Vertical");


        bool isWalking = movement.magnitude > 0f;
        animator.SetBool("isWalking", isWalking);

        if (isWalking)
        {
            Vector3 moveDirection = cameraTransform.forward * movement.z + cameraTransform.right * movement.x;
            moveDirection.y = 0f;

            Vector3 move = moveDirection.normalized * speed * Time.deltaTime;
            rb.MovePosition(rb.position + move);
        }
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
            speed = speed * 2;
        } else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            animator.SetBool("isSprinting", false);
            speed = speed / 2;
        }
    }

    private void CheckJump()
    {
        rb.AddForce(Vector3.up * 800f, ForceMode.Impulse);
    }

    private void CheckDance()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.F))
        {
            animator.SetTrigger("Dance");
        }
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
