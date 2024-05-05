using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelDoor : MonoBehaviour
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
        //Debug.Log("DoorClicked");
        if (GameManager.Instance.HasKeycard && Vector3.Distance(transform.position, player.position) < 5f)
        {
            Debug.Log("Door Clicked");
            GameManager.Instance.CompleteLevel();
        }
    }
}
