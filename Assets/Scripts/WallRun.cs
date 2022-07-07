using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    [SerializeField] PlayerMovement playerMovement;

   [Header("Movement")]
    [SerializeField] private Transform orientation;

    [Header("Detection")]
    [SerializeField] private float wallDistance = .5f;
    [SerializeField] private float minimumJumpHeight = 1.5f;

    [Header("Wall Running")]
    [SerializeField] private float wallRunGravity;
    [SerializeField] private float wallRunJumpForce;
    [SerializeField] private float wallRunSpeed;

    [Header("Camera")]
    [SerializeField] private Camera cam;
    [SerializeField] private float fov;
    [SerializeField] private float wallRunfov;
    [SerializeField] private float wallRunfovTime;
    [SerializeField] private float camTilt;
    [SerializeField] private float camTiltTime;


    public float tilt { get; private set; }

    private bool wallLeft = false;
    private bool wallRight = false;

    RaycastHit leftWallHit;
    RaycastHit rightWallHit;

    public Rigidbody rb;

    public bool isWallRunning = false;

    public bool wallRunStarted = false;

    public Vector3 wallForward;

    [SerializeField] private LayerMask whatIsWall;

     


    public bool CanWallRun()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minimumJumpHeight);
    }

    private void Start()
    {
    
    }

    public void CheckWall()
    {
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallDistance, whatIsWall);
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallDistance, whatIsWall);
    }

    private void Update()
    {
        CheckWall();

        if (CanWallRun())
        {
            if (wallLeft)
            {
                //Debug.Log("wall running on the left");
                
                    StartWallRun();                   
                
            }
            else if (wallRight)
            {
                //Debug.Log("wall running on the right");
                
                    StartWallRun();
                
            }
            else
            {
                StopWallRun();
            }
        }
        else
        {
            StopWallRun();
        }

        //if(Input.GetKey("d")){

            //Debug.Log("Pressed a or d");
        //}
    }
   
    void StartWallRun()
    {
        
        isWallRunning = true;        

        if(playerMovement.rb.velocity.y<0){
            
        }
        
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        //Debug.Log(rb.velocity.y);
        wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
            wallForward = -wallForward;

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, wallRunfov, wallRunfovTime * Time.deltaTime);

        if (wallLeft)
            tilt = Mathf.Lerp(tilt, -camTilt, camTiltTime * Time.deltaTime);
        else if (wallRight)
            tilt = Mathf.Lerp(tilt, camTilt, camTiltTime * Time.deltaTime);

        

        if (Input.GetKeyDown(KeyCode.Space))
        {
            
            if (wallLeft)
            {
                Vector3 wallRunJumpDirection = transform.up + leftWallHit.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(wallRunJumpDirection * wallRunJumpForce * 100, ForceMode.Force);
            }
            else if (wallRight)
            {
                Vector3 wallRunJumpDirection = transform.up + rightWallHit.normal;
                
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); 
                rb.AddForce(wallRunJumpDirection * wallRunJumpForce * 100, ForceMode.Force);
            }
        }   
    }

    

    // add wall run canceler if player speed is lower than 5
        
    void StopWallRun()
    {  
        isWallRunning = false;

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov, wallRunfovTime * Time.deltaTime);
        tilt = Mathf.Lerp(tilt, 0, camTiltTime * Time.deltaTime);
    }

    
}
