using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlingShotHandler : MonoBehaviour
{

    [Header("Line Renderers")]
    [SerializeField] private LineRenderer leftLineRenderer;
    [SerializeField] private LineRenderer rightLineRenderer;

    [Header("Line Positions")]
    [SerializeField] private Transform leftStartPosition;
    [SerializeField] private Transform rightStartPosition;
    [SerializeField] private Transform centerPosition;
    [SerializeField] private Transform idlePosition;

    [Header("Slingshot Stats")]
    [SerializeField] private float maxDistance = 3.5f;

    [Header("Scripts")]
    [SerializeField] private SlingShotArea slingShotArea;

    private Vector2 slingShotLinePosition;

    private bool clickedWithinArea;

    private void Awake()
    {
        leftLineRenderer.enabled = false;
        rightLineRenderer.enabled = false;
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && slingShotArea.isWithinSlingShotArea())
        {
            clickedWithinArea = true;
        }

        if (Mouse.current.leftButton.isPressed && clickedWithinArea)
        {
            DrawLine();
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            clickedWithinArea = false;
        }
    }

    private void DrawLine()
    {
        if (!leftLineRenderer.enabled && !rightLineRenderer.enabled)
        {
            leftLineRenderer.enabled = true;
            rightLineRenderer.enabled = true;
        }

        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        slingShotLinePosition = centerPosition.position + Vector3.ClampMagnitude(touchPosition - centerPosition.position, maxDistance);
        SetLines(slingShotLinePosition);
    }

    private void SetLines(Vector2 position)
    {
        leftLineRenderer.SetPosition(0, position);
        leftLineRenderer.SetPosition(1, leftStartPosition.position);
        rightLineRenderer.SetPosition(0, position);
        rightLineRenderer.SetPosition(1, rightStartPosition.position);
    }
}
