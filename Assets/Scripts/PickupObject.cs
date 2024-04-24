using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupObject : MonoBehaviour
{
    [SerializeField] private float throwForce = 600f;
    [SerializeField] private Transform player;

    public bool isHeld = false;
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
        if (!isHeld)
        {
            rb.isKinematic = true;
            transform.SetParent(player);
            transform.localPosition = Vector3.zero + Vector3.forward;
            isHeld = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isHeld)
        {
            isHeld = false;
            
        }
    }
}
