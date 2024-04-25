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


public class RaycastController : MonoBehaviour
{
    RaycastHit hit;
    GameObject securityCam;
    private bool isMainCamera;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward) * 10f;
        Debug.DrawRay(transform.position, forward, Color.red);
        if (Input.GetMouseButtonDown(0))
        {
            //DoorToggle();

        }

        if (Input.GetMouseButtonDown(0))
        {
            ChangeCamera();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Got to Keycode.Q");
            if (!isMainCamera)
            {
                Debug.Log("Got to here");
                securityCam.GetComponent<Camera>().enabled = false;
                gameObject.GetComponent<Camera>().enabled = true;
                isMainCamera = true;
            }
        }
    }

    public void ChangeCamera()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            if (hit.collider.CompareTag("Camera"))
            {
                securityCam = hit.collider.transform.GetChild(0).gameObject;
                securityCam.GetComponent<Camera>().enabled = true;
                gameObject.GetComponent<Camera>().enabled = false;
                isMainCamera = false;
            }
        }
    }

    //private void DoorToggle()
    //{
    //    if (Physics.Raycast(transform.position, transform.forward, out hit, 10f))
    //    {
    //        Debug.Log("Hit " + hit.collider.name);
    //        if (hit.collider.CompareTag("Door"))
    //        {
    //            Debug.Log("ClickedDoor");
    //            GameObject door = hit.collider.gameObject;

    //            //door.GetComponentInParent<DoorController>().ToggleDoor();

    //        }
    //    }

    //}
}
