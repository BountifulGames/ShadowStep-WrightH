using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupObject : MonoBehaviour
{
    [SerializeField] private float throwForce = 600f;
    [SerializeField] private float noiseRadius = 10f;
    [SerializeField] private Transform player;

    public bool isHeld = false;
    private bool hasLanded = true;
    private Rigidbody rb;

    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isHeld && Input.GetMouseButtonDown(1))
        {
            ThrowObject();
        }
    }

    void ThrowObject()
    {
        transform.SetParent(null);
        isHeld = false;
        rb.isKinematic = false;
        rb.AddForce(player.forward * throwForce);
    }

    private void OnMouseDown()
    {
        if (!isHeld && Vector3.Distance(player.position, transform.position) < 5f)
        {
            rb.isKinematic = true;
            transform.SetParent(player);
            transform.localPosition = Vector3.zero + Vector3.forward;
            isHeld = true;
            hasLanded = false;

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasLanded)
        {
            Debug.Log("Object landed");
            hasLanded = true;
            NotifyEnemies();
        }
    }

    private void NotifyEnemies()
    {
        Collider[] enemyColliders = Physics.OverlapSphere(transform.position, noiseRadius);
        foreach (Collider collider in enemyColliders)
        {
            Debug.Log("Collider found:" + collider.name);
            if (collider.CompareTag("Enemy"))
            {
                EnemyAI enemy = collider.GetComponent<EnemyAI>();
                enemy.InvestigateNoise(transform.position);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, noiseRadius);
    }
}
