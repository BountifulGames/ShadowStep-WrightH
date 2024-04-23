using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float chaseSpeed = 5.0f;
    [SerializeField] private float patrolSpeed = 2.0f;
    [SerializeField] private float sightDistance = 10.0f;
    [SerializeField] private float fieldOfViewAngle = 90f;
    [SerializeField] private Transform playerPos;

    private Vector3 lastKnownLocation;
    private EnemyController enemyController;
    private int currentWaypoint = 0;
    private bool isInvestigating = false;
    
    private enum EnemyState { Patrolling, Chasing, Investigating }
    private EnemyState enemyState;

    private void Awake()
    {
        enemyController = GetComponent<EnemyController>();
        enemyState = EnemyState.Patrolling;
    }
    void Update()
    {
        StateChange();
        LookForPlayer();

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
        Debug.Log("Patroling");
        enemyController.MoveTo(waypoints[currentWaypoint].position, patrolSpeed);
        if (Vector3.Distance(gameObject.transform.position, waypoints[currentWaypoint].position) < 1.0f)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        }

    }

    private void Chase()
    {
        StopCoroutine(PerformInvestigation());
        Debug.Log("Chasing");
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
        Debug.Log("Last known location " + lastKnownLocation.ToString());
        enemyController.MoveTo(lastKnownLocation, patrolSpeed);
        if (Vector3.Distance(gameObject.transform.position, lastKnownLocation) <= 1.5f && !isInvestigating)
        {
            Debug.Log("Beginning investigation");
            StartCoroutine(PerformInvestigation());
        }
    }

    private bool CanSeePlayer()
    {
        Vector3 playerDirection = playerPos.position - gameObject.transform.position;
        float playerAngle = Vector3.Angle(playerDirection, transform.forward);
        if (playerAngle < fieldOfViewAngle * 0.5f)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + transform.up, playerDirection.normalized, out hit, sightDistance))
            {
                Debug.DrawRay(transform.position, playerDirection.normalized, Color.red);

                Debug.Log("Raycast Hit " + hit.collider.name);
                return hit.transform == playerPos;
            }
        }
        return false;
    }

    private void LookForPlayer()
    {
        if (CanSeePlayer() && Vector3.Distance(playerPos.position, gameObject.transform.position) < sightDistance)
        {
            lastKnownLocation = playerPos.position;
            enemyState = EnemyState.Chasing;
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
