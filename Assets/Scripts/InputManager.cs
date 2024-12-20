using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static PlayerInput playerInput;
    private InputAction mousePositionAction;
    private InputAction mouseAction;
    public static Vector2 mousePosition;

    public static bool wasLeftPressed;
    public static bool wasLeftReleased;
    public static bool isLeftPressed;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        mousePositionAction = playerInput.actions["MousePosition"];
        mouseAction = playerInput.actions["Mouse"];
    }

    private void Update()
    {
        mousePosition = mousePositionAction.ReadValue<Vector2>();

        wasLeftPressed = mouseAction.WasPressedThisFrame();
        wasLeftReleased = mouseAction.WasReleasedThisFrame();
        isLeftPressed = mouseAction.IsPressed();
    }
}
