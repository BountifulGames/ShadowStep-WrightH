using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keycard : MonoBehaviour
{
    [SerializeField] private Transform player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        if (Vector3.Distance(transform.position, player.position) < 5f)
        {
            GameManager.Instance.CollectKeycard();
            Destroy(gameObject);
        }
    }
}
