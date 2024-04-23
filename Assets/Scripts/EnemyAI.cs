using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float chaseSpeed = 5.0f;
    [SerializeField] private float patrolSpeed = 2.0f;
    [SerializeField] private float sightDistance = 10.0f;
    [SerializeField] private float fieldOfViewAngle = 110f;
    private Vector3 lastKnownLocation;
    private EnemyController enemyController;
    private Transform playerPos;
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
        if (Vector3.Distance(gameObject.transform.position, waypoints[currentWaypoint].position) < 1.0f)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        }
        enemyController.MoveTo(waypoints[currentWaypoint].position, patrolSpeed);
    }

    private void Chase()
    {
        enemyController.MoveTo(playerPos.position, chaseSpeed);
        if (Vector3.Distance(gameObject.transform.position, playerPos.position) > sightDistance || !CanSeePlayer())
        {
            lastKnownLocation = playerPos.position;
            enemyState = EnemyState.Investigating;
        }
    }

    private void Investigate()
    {
        enemyController.MoveTo(lastKnownLocation, patrolSpeed);
        if (Vector3.Distance(gameObject.transform.position, lastKnownLocation) < 1f && !isInvestigating)
        {
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
                return hit.transform == playerPos;
            }
        }
        return false;
    }

    private IEnumerator PerformInvestigation()
    {
        isInvestigating = true;

        yield return new WaitForSeconds(5);

        isInvestigating = false;
        enemyState = EnemyState.Patrolling;
    }
}
