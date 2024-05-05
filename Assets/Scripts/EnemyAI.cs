using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private Slider detectionMeterSlider;
    [SerializeField] private Canvas enemyCanvas;
    [SerializeField] private float chaseSpeed = 5.0f;
    [SerializeField] private float patrolSpeed = 2.0f;
    [SerializeField] private float sightDistance = 10.0f;
    [SerializeField] private float fieldOfViewAngle = 130f;
    [SerializeField] private float detectionRate = 45f;
    [SerializeField] private float decreaseRate = 5f;
    [SerializeField] private Transform playerPos;

    private float detectionMeter = 0f;
    private float detectionMax = 100f;
    private float lastDetectionMeterValue = -1;
    private Animator animator;
    [SerializeField] private bool isWalking;
    private bool isChasing = false;

    [SerializeField] private bool isAlert = false;

    private Vector3 lastKnownLocation;
    private EnemyController enemyController;
    private int currentWaypoint = 0;
    private bool isInvestigating = false;
    [SerializeField] private CharacterController characterController;

    Coroutine waitCoroutine;
    
    private enum EnemyState { Patrolling, Chasing, Investigating, Waiting }
    private EnemyState enemyState;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        characterController = playerPos.gameObject.GetComponent<CharacterController>();
        enemyController = GetComponent<EnemyController>();
        enemyState = EnemyState.Patrolling;
    }
    void Update()
    {

        LookForPlayer();

        StateChange();

        UpdateDetectionUI();

        if (detectionMeter > 0 && !CanSeePlayer())
        {
            detectionMeter -= decreaseRate * Time.deltaTime;

            if (detectionMeter <= 0)
            {
                isAlert = false;
            }
        }

        if (isWalking)
        {
            animator.SetBool("isWalking", isWalking);
        }
        if (!isWalking)
        {
            animator.SetBool("isWalking", isWalking);
        }

        //Debug.Log("Current Waypoint: " + currentWaypoint +
        //    "\nDistance to waypoint: " + Vector3.Distance(transform.position, waypoints[currentWaypoint].position));

    }

    private void StateChange()
    {
        switch (enemyState)
        {
            case EnemyState.Patrolling:
                if (waitCoroutine != null)
                {
                    StopCoroutine(waitCoroutine);
                }
                Patrol();
                break;
            case EnemyState.Chasing:
                Chase();
                break;
            case EnemyState.Investigating:
                Investigate();
                break;
            case EnemyState.Waiting:
                if (waitCoroutine == null)
                {
                    waitCoroutine = StartCoroutine(WaitAtWaypoint());
                }
                break;
        }
    }

    private void Patrol()
    {
        if (enemyState == EnemyState.Patrolling)
        {
            if (Vector3.Distance(transform.position, waypoints[currentWaypoint].position) > 1.0f)
            {
                if (!isWalking)
                {
                    isWalking = true;
                    animator.SetBool("isWalking", true);
                }
                enemyController.MoveTo(waypoints[currentWaypoint].position, patrolSpeed);
            }
            else if (isWalking)
            {
                // Arrived at waypoint, transition to waiting state
                isWalking = false;
                animator.SetBool("isWalking", false);
                enemyState = EnemyState.Waiting;  // Transition to waiting state
            }
            
        }
    }

    private IEnumerator WaitAtWaypoint()
    {
        yield return new WaitForSeconds(3);  
        currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        isWalking = true;
        animator.SetBool("isWalking", true);
        enemyState = EnemyState.Patrolling;  
        waitCoroutine = null;
    }

    private void Chase()
    {
        StopCoroutine(PerformInvestigation());
        //Debug.Log("Chasing");
        if (!isWalking)
        {
            isWalking = true;
            animator.SetBool("isWalking", true);
        }
        enemyController.MoveTo(playerPos.position, chaseSpeed);
        if (Vector3.Distance(gameObject.transform.position, playerPos.position) > sightDistance || !CanSeePlayer())
        {
            lastKnownLocation = playerPos.position;
            isChasing = false;
            enemyState = EnemyState.Investigating;
        }

        if (Vector3.Distance(gameObject.transform.position, playerPos.position) < 1.0f)
        {
            GameManager.Instance.GameOver();
        }
    }

    private void Investigate()
    {
        //Debug.Log("Investigating");
        //Debug.Log("Last known location " + lastKnownLocation.ToString());
        enemyController.MoveTo(lastKnownLocation, patrolSpeed);
        if (Vector3.Distance(gameObject.transform.position, lastKnownLocation) <= 1.5f && !isInvestigating)
        {
            //Debug.Log("Beginning investigation");
            isWalking = false;
            StartCoroutine(PerformInvestigation());
            isChasing = false;
        }
    }

    public void InvestigateNoise(Vector3 noisePosition)
    {
        lastKnownLocation = noisePosition; // Set the noise position as the last sighting
        enemyState = EnemyState.Investigating;
    }

    
    private bool CanSeePlayer()
    {
        float playerHeight = characterController.height;
        Vector3 eyeLevel = transform.position + transform.up * 0.8f; 
        Vector3 targetPosition = playerPos.position + transform.up * 0.3f;
        Vector3 directionToPlayer = targetPosition - eyeLevel;

        float playerAngle = Vector3.Angle(directionToPlayer, transform.forward);
        if (playerAngle < fieldOfViewAngle * 0.5f) 
        {
            RaycastHit hit;
            if (Physics.Raycast(eyeLevel, directionToPlayer.normalized, out hit, sightDistance))
            {
                Debug.DrawRay(eyeLevel, directionToPlayer.normalized * sightDistance, hit.transform == playerPos ? Color.green : Color.red);
                if (hit.transform == playerPos)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        return false; 
    }

    private void UpdateDetectionUI()
    {
        if (detectionMeter != lastDetectionMeterValue)
        {
            if (detectionMeterSlider != null)
            {
                if (detectionMeter >= 0)
                {
                    detectionMeterSlider.gameObject.SetActive(true);

                    detectionMeterSlider.value = detectionMeter;

                    detectionMeterSlider.fillRect.GetComponent<Image>().color = Color.Lerp(Color.green, Color.red, detectionMeter / detectionMax);
                }
                else
                {
                    detectionMeterSlider.gameObject.SetActive(false);
                }

            }
            lastDetectionMeterValue = detectionMeter;
        }


        enemyCanvas.transform.LookAt(playerPos.position);
    }

    private void LookForPlayer()
    {
        if (CanSeePlayer() && Vector3.Distance(playerPos.position, gameObject.transform.position) < sightDistance)
        {
            detectionMeter += detectionRate * Time.deltaTime;
            detectionMeter = Mathf.Clamp(detectionMeter, 0, detectionMax);
            if (detectionMeter >= detectionMax && !isChasing)
            {
                GameManager.Instance.IncreaseDetect();
                isChasing = true;
                lastKnownLocation = playerPos.position;
                enemyState = EnemyState.Chasing;

            }
        }

    }

    public void IncreaseAlert(int alertLevel)
    {
        switch (alertLevel)
        {
            case 1:
                Debug.Log("Alert Level 1");
                sightDistance = 5.0f;  
                chaseSpeed = 3.0f;
                detectionRate = 45f;
                break;
            case 2:
                Debug.Log("Alert Level 2");

                sightDistance = 10.0f;  
                chaseSpeed = 4.0f;
                detectionRate = 65f;
                break;
            case 3:
                Debug.Log("Alert Level 3");

                sightDistance = 15.0f;
                detectionRate = 75f;
                break;
            default:
                Debug.Log("Unsupported alert level!");
                break;
        }

    }

    private void ToggleAlert()
    {
        if (!isAlert)
        {
            isAlert = !isAlert;
            sightDistance = sightDistance * 1.5f;
            fieldOfViewAngle = fieldOfViewAngle + 20f;
        }
        else if (isAlert)
        {
            isAlert = !isAlert;
            sightDistance = sightDistance / 1.5f;
            fieldOfViewAngle = fieldOfViewAngle - 20f;
        }
    }

    private IEnumerator PerformInvestigation()
    {
        isInvestigating = true;

        yield return new WaitForSeconds(8);

        isInvestigating = false;
        enemyState = EnemyState.Patrolling;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(gameObject.transform.position, sightDistance);
        Vector3 fovLine1 = Quaternion.AngleAxis(fieldOfViewAngle / 2, transform.up) * transform.forward * sightDistance;
        Vector3 fovLine2 = Quaternion.AngleAxis(-fieldOfViewAngle / 2, transform.up) * transform.forward * sightDistance;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + fovLine1);
        Gizmos.DrawLine(transform.position, transform.position + fovLine2);

        if (CanSeePlayer())
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, playerPos.position);
        }

    }
}
