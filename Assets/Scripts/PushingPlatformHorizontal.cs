using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushingPlatformHorizontal : MonoBehaviour
{
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] float pushForce = 25f;
    public Transform cam;
    private float distance=10f;
    private int layerMaskPushingPlatform;

    Vector3 pushingDirection;

    bool pushing;

    void Start()
    {
        layerMaskPushingPlatform = 1 << 13;
    }

    // Update is called once per frame
    void Update()
    {
        if(Physics.Raycast(cam.transform.position, -cam.transform.right, out RaycastHit raycastHitLeft, distance, layerMaskPushingPlatform)){
            
            Debug.Log("Left");

            pushingDirection = cam.transform.right;

            pushing= true;
        }
        else if(Physics.Raycast(cam.transform.position, cam.transform.right, out RaycastHit raycastHitRight, distance, layerMaskPushingPlatform)){

            Debug.Log("Right");

            pushingDirection = -cam.transform.right;

            pushing = true;
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
        playerMovement.rb.AddForce(pushingDirection * pushForce, ForceMode.Impulse);
    }
}
