using System.Diagnostics;
using System.Net.Mime;
using System.Data.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{

    float playerHeight = 2f;

    [SerializeField] WallRun wallRun;

    [SerializeField] Transform orientation;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 6f;

    [SerializeField] float wallRunSpeed;
    [SerializeField] public float airMultiplier = 0.4f;

    [SerializeField] public float hookMultiplier = 0.1f;
    public float movementMultiplier = 10f;

    [Header("Sprinting")]
    [SerializeField] float walkSpeed = 4f;
    [SerializeField] float sprintSpeed = 6f;
    [SerializeField] float acceleration = 10f;

    [Header("Jumping")]
    public float jumpForce = 30f;

    [Header("Keybinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Drag")]
    [SerializeField] float groundDrag = 6f;
    [SerializeField] float airDrag = 2f;

    public float horizontalMovement;
    public float verticalMovement;

    [Header("Ground Detection")]
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;
    [SerializeField] float groundDistance = 0.2f;
    [SerializeField] public bool isGrounded { get; private set; }
    [SerializeField] public bool isSlidingPlatformed { get; private set; }
    [SerializeField] float slidingPlatformDistance = 0.5f;

    
    [Header("Wall run")]
    Vector3 wallRunDirection;


    Vector3 moveDirection;
    Vector3 slopeMoveDirection;
    public Rigidbody rb;
    RaycastHit slopeHit;

    [Header("Playerlook")]
    [SerializeField] private float sensX = 100f;
    [SerializeField] private float sensY = 100f;

    [SerializeField] public Transform cam;

    public Quaternion TargetRotation { private set; get; }
    float mouseX;
    float mouseY;

    float multiplier = 0.01f;

    float xRotation;
    float yRotation;

    [Header("Gravity")]

    [SerializeField] public float gravity;
    [SerializeField] public float currentGravity;
    [SerializeField] public float constantGravity;
    [SerializeField] public float maxGravity;
    private Vector3 gravityDirection;
    private Vector3 gravityMovement;

    //public bool isOutOfRange = false;
    public State state;
    public bool sliding;

    //[SerializeField] private ParticleSystem speedLinesParticleSystem;

    private Vector3 characterVelocityMomentum;

    [Header("Crosshair")]

    [SerializeField] Image crosshair;
    public enum State{
        Normal,
        Wallrunning,
        HookshotThrown,
        HookshotFlyingPlayer
    }

    private void Awake(){
        state = State.Normal;
        gravityDirection = Vector3.down;
        
    }
    

    
    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2 + 0.5f))
        {
            if (slopeHit.normal != Vector3.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }
    public Vector3 GetSlopeMoveDirection(Vector3 direction){
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        TargetRotation = transform.rotation;

        
        
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        isSlidingPlatformed = Physics.CheckSphere(groundCheck.position, slidingPlatformDistance, groundMask);
        MyInput();
        
        //Gravity
        
        
        
        CalculateGravity();

        switch(state)
        {
        default:
        case State.Normal:
            if(!wallRun.isWallRunning)
                UseGravity(); 
            ControlDrag();
            ControlSpeed();
            HandleCharacterLook();
            Jump();
            //HandleHookshotStart();
            break;
        case State.HookshotThrown:
            
            HandleCharacterLook();
            Jump();
            break;    
        case State.HookshotFlyingPlayer:
            //HookshotCanceler();
            HandleCharacterLook();
            
            break;
        }        

        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
        
    }

    private void FixedUpdate()
    {
       
        switch(state)
        {
        default:
        case State.Normal:  
            
            MovePlayer();
            break;
        case State.HookshotThrown:
            MovePlayer();         
            //HandleHookshotThrow();
            break;
        case State.HookshotFlyingPlayer:
            
            //HandleHookshotMovement();
            break;
        }

    }

    void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;
    }

    public void Jump()
    {
        
        if (TestInputJump() && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
        else if (TestInputJump() && (state == State.HookshotFlyingPlayer))
        {
            
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce * 1.5f, ForceMode.Impulse);
        }
        
        
    }

    void ControlSpeed()
    {
        if (Input.GetKey(sprintKey) && isGrounded || Input.GetKey(sprintKey) && wallRun.isWallRunning)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration * Time.deltaTime);
        }
    }

    void ControlDrag()
    {
        if (isGrounded)
        {
            characterVelocityMomentum.y = 0;
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = airDrag;
        }
    }

    void MovePlayer()
    {
        rb.velocity += characterVelocityMomentum;

        if (isGrounded && !OnSlope())
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if (isGrounded && OnSlope())
        {
            rb.AddForce(slopeMoveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }   
        else if (!isGrounded && wallRun.isWallRunning)
        {
            rb.AddForce(wallRun.wallForward * moveSpeed * movementMultiplier * airMultiplier * 1.75f, ForceMode.Acceleration);
            
        }
        else if (!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
        }

        //hook,
        
        if(characterVelocityMomentum.magnitude >= 0f){
            float momentumDrag = 3f;
            characterVelocityMomentum -= characterVelocityMomentum * momentumDrag * Time.fixedDeltaTime;
            if(characterVelocityMomentum.magnitude < .0f){
                characterVelocityMomentum = Vector3.zero;
            }
        }
        
    }
        
    public void HandleCharacterLook(){
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");
         
        yRotation += mouseX * sensX * multiplier;
        xRotation -= mouseY * sensY * multiplier;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.transform.rotation = Quaternion.Euler(xRotation, yRotation, wallRun.tilt);

        if(!wallRun.isWallRunning){
        orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);
        }
            
    }    

    
    public void UseGravity(){
        rb.AddForce(gravityMovement * 1.1f, ForceMode.Impulse);
    
    }

    private void CalculateGravity(){
        if(wallRun.isWallRunning || state != State.Normal){
            currentGravity = 0;
        }
        if(isGrounded){
            currentGravity = constantGravity;
        }
        else {
            if(currentGravity > maxGravity){
                currentGravity -= gravity * Time.deltaTime;
            }   
        }
        gravityMovement = gravityDirection * -currentGravity * Time.deltaTime * 8;
    }
    
    public void ResetTargetRotation()
    {
        TargetRotation = Quaternion.LookRotation(transform.forward, Vector3.up);
    }
        
    
    
    private bool TestInputJump(){
        return Input.GetKeyDown(jumpKey);
    }
    

    

}
