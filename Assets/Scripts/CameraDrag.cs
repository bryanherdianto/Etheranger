using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;



public class CameraDrag : MonoBehaviour
{
    private Vector3 originalPosition;
    private Vector3 difference;
    private Camera idleCam;

    private bool isDragging;
    private Vector3 GetMousePosition => idleCam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    
    private void Awake()
    {
        idleCam = Camera.main;
    }

    public void OnDrag(InputAction.CallbackContext context)
    {
        if (context.started) originalPosition = GetMousePosition;
        isDragging = context.started || context.performed;
    }

    private void LateUpdate()
    {
        if (!isDragging) return;
        
        difference = GetMousePosition - transform.position;
        transform.position = originalPosition - difference;      
    }


}
