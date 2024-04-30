using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private float fieldOfViewAngle = 90f;
    [SerializeField] private float detectionRate = 20f;
    [SerializeField] private float decreaseRate = 5f;
    [SerializeField] private float raycastInterval = 0.01f;
    [SerializeField] private Transform playerPos;

    private float detectionMeter = 0f;
    private float detectionMax = 100f;
    private int detectCount = 0;
    private float nextRaycast = 0f;
    private float lastDetectionMeterValue = -1;

    private Vector3 lastKnownLocation;
    private EnemyController enemyController;
    private int currentWaypoint = 0;
    private bool isInvestigating = false;
    [SerializeField] private CharacterController characterController;
    
    private enum EnemyState { Patrolling, Chasing, Investigating }
    private EnemyState enemyState;

    private void Awake()
    {
        enemyController = GetComponent<EnemyController>();
        enemyState = EnemyState.Patrolling;
    }
    void Update()
    {

        LookForPlayer();

        StateChange();

        UpdateDetectionUI();

    }

    private void StateChange()
    {
        switch (enemyState)
        {
            case EnemyState.Patrolling:
                Patrol();
                break;
            case EnemyState.Chasing:
                Chase();
                break;
            case EnemyState.Investigating:
                Investigate();
                break;
        }
    }

    private void Patrol()
    {
        //Debug.Log("Patroling");
        enemyController.MoveTo(waypoints[currentWaypoint].position, patrolSpeed);
        if (Vector3.Distance(gameObject.transform.position, waypoints[currentWaypoint].position) < 1.0f)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        }

    }

    private void Chase()
    {
        StopCoroutine(PerformInvestigation());
        //Debug.Log("Chasing");
        enemyController.MoveTo(playerPos.position, chaseSpeed);
        if (Vector3.Distance(gameObject.transform.position, playerPos.position) > sightDistance || !CanSeePlayer())
        {
            lastKnownLocation = playerPos.position;
            enemyState = EnemyState.Investigating;
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
            StartCoroutine(PerformInvestigation());
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
        nextRaycast = Time.deltaTime + raycastInterval;
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
            if (detectionMeter > detectionMax)
            {
                detectCount++;
                //detectionMeter = 0;
                lastKnownLocation = playerPos.position;
                enemyState = EnemyState.Chasing;

                if (detectCount >= 3)
                {
                    //Game over script
                }
            }
        }
        else
        {
            if (detectionMeter > 0)
            {
                detectionMeter -= decreaseRate * Time.deltaTime;
            }
        }
    }

    private IEnumerator PerformInvestigation()
    {
        isInvestigating = true;

        yield return new WaitForSeconds(5);

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
