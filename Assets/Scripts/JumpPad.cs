using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] float jumpForce;
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player"){
            playerMovement.rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }
}
