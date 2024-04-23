using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveTo(Vector3 destination, float speed)
    {
        agent.speed = speed;
        agent.SetDestination(destination);
    }

    public void Stop()
    {
        agent.isStopped = true;
    }
}
