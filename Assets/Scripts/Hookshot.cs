using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hookshot : MonoBehaviour
{
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] WallRun wallRun;
    [SerializeField] private Transform hookshotTransform;
    private float hookshotSize;
    private Vector3 hookShotPosition;

    [SerializeField] Camera cam;

    [Header("Crosshair")]

    [SerializeField] Image crosshair;

    private Color basic;

    private Color duringHookshot = new Color32(82,82,82,255);

    private Color outOfDistanceShot = new Color32(166,38,38,255);

    int layerMaskPlayer;


    public bool isHooking = false;

     [SerializeField] private float fov;
    [SerializeField] private float hookFov;
    [SerializeField] private float hookFovTime;

    
    private void Awake(){
        hookshotTransform.gameObject.SetActive(false);
    }
    
    void Start()
    {
        basic = crosshair.GetComponent<Image>().color;
        layerMaskPlayer = 1 << 10;
        layerMaskPlayer = ~layerMaskPlayer;

    }

    
    void Update()
    {
        switch(playerMovement.state)
        {
        default:
        case PlayerMovement.State.Normal:
                
            HandleHookshotStart();
            break;
        case PlayerMovement.State.HookshotThrown:
            
            break;    
        case PlayerMovement.State.HookshotFlyingPlayer:
            HookshotCanceler();       
            break;
        }       
    }

    void FixedUpdate(){
        switch(playerMovement.state)
        {
        default:
        case PlayerMovement.State.Normal:  
            
            break;
        case PlayerMovement.State.HookshotThrown:
                   
            HandleHookshotThrow();
            break;
        case PlayerMovement.State.HookshotFlyingPlayer:
            
            HandleHookshotMovement();
            break;
        }
    }

    private void HandleHookshotStart(){
        
        
        float hookshotMaxDistance = 75f;
        if(TestInputDownHookshot()){
           if(Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit raycastHit, hookshotMaxDistance, layerMaskPlayer)){ // hits something
                
                if(raycastHit.collider.gameObject.layer == 7){
                    //Debug.Log("Check")
                    //DebugHitPointTransform.position = raycastHit.point
                    hookShotPosition = raycastHit.point;
                    hookshotSize = 0f;
                    hookshotTransform.gameObject.SetActive(true);
                    hookshotTransform.localScale = Vector3.zero;
                    playerMovement.state = PlayerMovement.State.HookshotThrown;
                }
                else{
                    Debug.Log(raycastHit.collider.gameObject.layer);
                    StartCoroutine(ChangeColorToBasic());
                }
                
            }
            else{
                    StartCoroutine(ChangeColorToBasic());
            }

        }
    }

    private void HandleHookshotThrow(){
        hookshotTransform.LookAt(hookShotPosition);

        float hookshotThrowSpeed = 250f;

        hookshotSize += hookshotThrowSpeed * Time.fixedDeltaTime;

        hookshotTransform.localScale = new Vector3(1,1,hookshotSize);

        if(hookshotSize >= Vector3.Distance(transform.position, hookShotPosition)){
            playerMovement.state = PlayerMovement.State.HookshotFlyingPlayer;
        }
    }

    private void HandleHookshotMovement(){
        isHooking = true;
        ChangeColorToHooking();

        hookshotTransform.LookAt(hookShotPosition);
        Vector3 hookShotDirection = (hookShotPosition - transform.position).normalized;

        float hookshotSpeedMin = 9f;
        float hookshotSpeedMax = 11f;
        float hookShotSpeed = Mathf.Clamp(Vector3.Distance(transform.position, hookShotPosition), hookshotSpeedMin, hookshotSpeedMax);

        // move to that point.
        playerMovement.rb.AddForce(hookShotDirection * hookShotSpeed * playerMovement.movementMultiplier * playerMovement.hookMultiplier * 10f, ForceMode.Force);

        hookshotSize -= hookShotSpeed * playerMovement.hookMultiplier* Time.fixedDeltaTime;
        hookshotTransform.localScale = new Vector3(1, 1, hookshotSize);

        float reachedHookshotPositionDistance = 5f; // distance that will cancel the hook when we get that point.
        if(Vector3.Distance(transform.position, hookShotPosition) < reachedHookshotPositionDistance){
            // Reached Hookshot Position
            StopHookshot();
        }
        if(wallRun.isWallRunning){           
            StopHookshot();
        } 
    }

    private void HookshotCanceler(){
        
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, hookFov, hookFovTime * Time.deltaTime);

        if(TestInputDownHookshot()){ 
            StopHookshot();
        }
        
        if(TestInputJump()){ 
            playerMovement.Jump();
            StopHookshot();
        }   
        
    }

    private void StopHookshot(){
        
        playerMovement.state = PlayerMovement.State.Normal;
        hookshotTransform.gameObject.SetActive(false);
        
        crosshair.rectTransform.sizeDelta = new Vector2(60,60);
        crosshair.GetComponent<Image>().color = basic;
        //speedLinesParticleSystem.Stop();
    }

    private bool TestInputDownHookshot(){
        return Input.GetKeyDown(KeyCode.E);
    }
    private bool TestInputJump(){
        return Input.GetKeyDown(KeyCode.Space);
    }

    IEnumerator ChangeColorToBasic(){

        ChangeColorToFail();
        
        yield return new WaitForSeconds(0.075f);

        crosshair.GetComponent<Image>().color = basic;
    }

    void ChangeColorToFail(){
        crosshair.GetComponent<Image>().color = outOfDistanceShot;
    }

    void ChangeColorToHooking(){
        crosshair.rectTransform.sizeDelta = new Vector2(36,36);
        crosshair.GetComponent<Image>().color = duringHookshot;
    }
}
