using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] Transform cameraPosition = null;
    void FixedUpdate()
    {
        transform.position = cameraPosition.position;
        
    }
}
