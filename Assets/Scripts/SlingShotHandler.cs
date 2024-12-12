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
    [SerializeField] private float shotForce = 10f;

    [Header("Scripts")]
    [SerializeField] private SlingShotArea slingShotArea;

    [Header("Bird")]
    [SerializeField] private AngryBird angryBirdPrefab;
    [SerializeField] private float angryBirdPositionOffset = 2f;
    [SerializeField] private float timeBetweenBirdRespawns = 2f;


    private Vector2 slingShotLinePosition;
    private Vector2 direction;
    private Vector2 directionNormalized;

    private bool clickedWithinArea;
    private bool birdOnSlingshot;

    private AngryBird spawnedAngryBird;

    private void Awake()
    {
        leftLineRenderer.enabled = false;
        rightLineRenderer.enabled = false;

        SpawnAngryBird();
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && slingShotArea.isWithinSlingShotArea())
        {
            clickedWithinArea = true;
        }

        if (Mouse.current.leftButton.isPressed && clickedWithinArea && birdOnSlingshot)
        {
            DrawLine();
            PositionAndRotateAngryBird();
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame && birdOnSlingshot)
        {
            if(GameManager.instance.HasEnoughBirds())
            {
                clickedWithinArea = false;
                birdOnSlingshot = false;

                GameManager.instance.UseShot();
                
                spawnedAngryBird.LaunchBird(direction, shotForce);

                SetLines(centerPosition.position);

                if(GameManager.instance.HasEnoughBirds())
                {
                    StartCoroutine(SpawnAngryBirdAfterTime());
                }
            }
        }
    }

    #region Slingshot Methods
    private void DrawLine()
    {
        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        slingShotLinePosition = centerPosition.position + Vector3.ClampMagnitude(touchPosition - centerPosition.position, maxDistance);
        SetLines(slingShotLinePosition);
        direction = (Vector2)centerPosition.position - slingShotLinePosition;
        directionNormalized = direction.normalized;
    }

    private void SetLines(Vector2 position)
    {
        if (!leftLineRenderer.enabled && !rightLineRenderer.enabled)
        {
            leftLineRenderer.enabled = true;
            rightLineRenderer.enabled = true;
        }

        leftLineRenderer.SetPosition(0, position);
        leftLineRenderer.SetPosition(1, leftStartPosition.position);

        rightLineRenderer.SetPosition(0, position);
        rightLineRenderer.SetPosition(1, rightStartPosition.position);
    }

    #endregion

    #region manuk nesu Methods

    private void SpawnAngryBird()
    {
        SetLines(idlePosition.position);

        Vector2 dir = (centerPosition.position - idlePosition.position).normalized;
        Vector2 spawnPosition = (Vector2)idlePosition.position + dir * angryBirdPositionOffset;
        spawnedAngryBird = Instantiate(angryBirdPrefab, spawnPosition, Quaternion.identity);
        spawnedAngryBird.transform.right = dir;

        birdOnSlingshot = true;
    }

    private void PositionAndRotateAngryBird()
    {
        spawnedAngryBird.transform.position = slingShotLinePosition + directionNormalized * angryBirdPositionOffset;
        spawnedAngryBird.transform.right = directionNormalized;
    }

    private IEnumerator SpawnAngryBirdAfterTime()
    {
        yield return new WaitForSeconds(timeBetweenBirdRespawns);
        SpawnAngryBird();
    }
    #endregion
}
