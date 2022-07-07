using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushingPlatformUp : MonoBehaviour
{
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] float pushForce = 50f;
    public Transform Player;
    private float distance=15f;
    private int layerMaskPushingPlatform;

    bool pushing;

    void Start()
    {
        layerMaskPushingPlatform = 1 << 11;
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Physics.Raycast(Player.transform.position, -Player.transform.up, out RaycastHit raycastHit, distance, layerMaskPushingPlatform)){
            
            pushing= true;
            
            playerMovement.currentGravity = 0;
            
        }
        else{
            pushing = false;
        }
    }

    void FixedUpdate()
    {
        if(pushing)
            Push();
    }

    private void Push(){
        playerMovement.rb.AddForce(transform.up * pushForce, ForceMode.Acceleration);
    }
}
