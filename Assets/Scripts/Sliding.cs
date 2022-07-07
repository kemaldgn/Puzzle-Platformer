using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform player;
    private Rigidbody rb;
    private PlayerMovement playerMovement;

    [Header("Sliding")]
    public float maxSlideTime;
    public float slideForce;
    private float slideTimer;

    public float slideYScale;
    private float startYScale;

    [Header("Input")]
    public KeyCode slideKey = KeyCode.LeftControl;
    private float horizontalInput;
    private float verticalInput;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerMovement>();

        startYScale = player.localScale.y;
    }

    private void Update()
    {

        if (Input.GetKeyDown(slideKey) && (playerMovement.horizontalMovement != 0 || playerMovement.verticalMovement != 0))
            StartSlide();

        if (Input.GetKeyUp(slideKey) && playerMovement.sliding)
            StopSlide();
    }

    private void FixedUpdate()
    {
        if (playerMovement.sliding)
            SlidingMovement();
    }

    private void StartSlide()
    {
        playerMovement.sliding = true;

        player.localScale = new Vector3(player.localScale.x, slideYScale, player.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        slideTimer = maxSlideTime;
    }

    private void SlidingMovement()
    {
        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // sliding normal
        if(!playerMovement.OnSlope() || rb.velocity.y > -0.1f)
        {
            rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

            slideTimer -= Time.deltaTime;
        }

        // sliding down a slope
        /*else
        {
            rb.AddForce(playerMovement.GetSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);
        }
        */
        if (slideTimer <= 0)
            StopSlide();
    }

    private void StopSlide()
    {
        playerMovement.sliding = false;

        player.localScale = new Vector3(player.localScale.x, startYScale, player.localScale.z);
    }
}
