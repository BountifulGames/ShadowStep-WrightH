using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaycastController : MonoBehaviour
{
    RaycastHit hit;
    [SerializeField] private GameObject securityCam;
    [SerializeField] private Slider cameraSlider;
    [SerializeField] private float hackingRate = 20f;
    private bool isMainCamera = true;
    private float hackingPercent = 0f;
    private Coroutine hackingCoroutine;




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

        if (Input.GetMouseButtonDown(0) && isMainCamera)
        {
            ChangeCamera();
        }
        if (Input.GetKeyDown(KeyCode.Q) && !isMainCamera)
        {
            StopCoroutine(hackingCoroutine);
            GameManager.Instance.StopHacking();
            ResetToMainCamera();
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            GameManager.Instance.IncreaseDetect();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("has keycard: " + GameManager.Instance.HasKeycard);
        }
    }

    public void ChangeCamera()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hit, 10f))
        {
            if (hit.collider.CompareTag("Camera"))
            {
                if (hackingCoroutine != null)
                {
                    StopCoroutine(hackingCoroutine);
                }
                hackingCoroutine = StartCoroutine(StartHacking(hit));

            }
        }
    }

    private void ResetToMainCamera()
    {
        if (securityCam != null)
        {
            securityCam.GetComponent<Camera>().enabled = false;
        }
        gameObject.GetComponent<Camera>().enabled = true;
        isMainCamera = true;

        if (hackingCoroutine != null)
        {
            StopCoroutine (hackingCoroutine);
            hackingCoroutine = null;
        }
        hackingPercent = 0f;
        cameraSlider.gameObject.SetActive(false);
    }

    private IEnumerator StartHacking(RaycastHit hit)
    {
        isMainCamera = false;
        GameManager.Instance.StartHacking();
        hackingPercent = 0f;
        cameraSlider.gameObject.SetActive(true);
        while (hackingPercent < 100f)
        {
            hackingPercent += hackingRate * Time.deltaTime;
            hackingPercent = Mathf.Clamp(hackingPercent, 0, 101f);
            cameraSlider.value = hackingPercent;

            Debug.Log("Hacking Percent: " + hackingPercent);
            if (hackingPercent >= 100f)
            {
                GameManager.Instance.StopHacking();
                securityCam = hit.collider.transform.GetChild(0).gameObject;
                securityCam.GetComponent<Camera>().enabled = true;
                gameObject.GetComponent<Camera>().enabled = false;
                cameraSlider.gameObject.SetActive(false);
                yield break;
            }
            yield return null;
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
