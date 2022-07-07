using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    [SerializeField] Transform orientation;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 6f;
    [SerializeField] float verticalSpeed = 5f;
    float movementMultiplier = 10f;

    [Header("Sprinting")]
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float acceleration;

    float horizontalMovement;
    float verticalMovement;
    Vector3 moveDirection;
    [SerializeField] Rigidbody rb;
 

    [Header("Playerlook")]
    [SerializeField] private float sensX = 100f;
    [SerializeField] private float sensY = 100f;
    [SerializeField] public Transform cam;
    float mouseX;
    float mouseY;
    float multiplier = 0.01f;
    float xRotation;
    float yRotation;
    //[SerializeField] private ParticleSystem speedLinesParticleSystem;

    bool goUp;
    bool goDown;


    [Header("Keybindings")]
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] KeyCode goUpKey = KeyCode.Space;
    [SerializeField] KeyCode goDownKey = KeyCode.LeftControl;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; 
    }

    private void Update()
    {
        MyInput();                 
        ControlSpeed();
        ControlDrag();
        HandleCharacterLook();
        ControlVerticalMovement();
            
    }        
        
    

    private void FixedUpdate()
    {   
        MovePlayer();
        if(goUp)
            MoveUp();
        if(goDown)
            MoveDown();
    }

    void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;
        
    }

    void ControlSpeed()
    {
        if (Input.GetKey(sprintKey))
        {
            moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration * Time.deltaTime);
        }
    }

    void ControlVerticalMovement(){
        
        if(Input.GetKey(goUpKey))
            goUp = true;
        else
            goUp = false;

        if(Input.GetKey(goDownKey))
            goDown = true;
        else
            goDown = false;
        
    }

    void ControlDrag()
    {
        
        rb.drag = 3f;
        
    }

    void MoveUp(){
         rb.AddForce(Vector3.up.normalized * verticalSpeed * movementMultiplier, ForceMode.Acceleration);
    }

    void MoveDown(){
        rb.AddForce(Vector3.down.normalized * verticalSpeed * movementMultiplier, ForceMode.Acceleration);
    }
    

    void MovePlayer()
    { 
        rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
    }
        
    public void HandleCharacterLook(){
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");
         
        yRotation += mouseX * sensX * multiplier;
        xRotation -= mouseY * sensY * multiplier;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);
            
    }    
}
