using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

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
    [SerializeField] private Transform elasticTransform;

    [Header("Slingshot Stats")]
    [SerializeField] private float maxDistance = 3.5f;
    [SerializeField] private float shotForce = 10f;
    [SerializeField] private float elasticDivider = 1.2f;
    [SerializeField] private AnimationCurve elasticCurve;
    [SerializeField] private float maximumAnimationTime = 1f;

    [Header("Scripts")]
    [SerializeField] private SlingShotArea slingShotArea;
    [SerializeField] private CameraManager cameraManager;

    [Header("Bird")]
    [SerializeField] private AngryBird angryBirdPrefab;
    [SerializeField] private float angryBirdPositionOffset = 2f;
    [SerializeField] private float timeBetweenBirdRespawns = 2f;

    [Header("Sound")]
    [SerializeField] private AudioClip slingshotPullSound;
    [SerializeField] private AudioClip slingshotReleaseSound;

    private Vector2 slingShotLinePosition;
    private Vector2 direction;
    private Vector2 directionNormalized;

    private bool clickedWithinArea;
    private bool birdOnSlingshot;

    private AngryBird spawnedAngryBird;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        leftLineRenderer.enabled = false;
        rightLineRenderer.enabled = false;

        SpawnAngryBird();
    }

    private void Update()
    {
        if (InputManager.wasLeftPressed && slingShotArea.isWithinSlingShotArea())
        {
            clickedWithinArea = true;

            if (birdOnSlingshot)
            {
                SoundManager.instance.PlayClip(slingshotPullSound, audioSource);
                cameraManager.SwitchToFollowCam(spawnedAngryBird.transform);
            }
        }

        if (InputManager.isLeftPressed && clickedWithinArea && birdOnSlingshot)
        {
            DrawLine();
            PositionAndRotateAngryBird();
        }

        if (InputManager.wasLeftReleased && birdOnSlingshot && clickedWithinArea)
        {
            if(GameManager.instance.HasEnoughBirds())
            {
                clickedWithinArea = false;
                birdOnSlingshot = false;

                GameManager.instance.UseShot();
                
                spawnedAngryBird.LaunchBird(direction, shotForce);

                SoundManager.instance.PlayClip(slingshotReleaseSound, audioSource);

                AnimateSlingshot();

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
        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(InputManager.mousePosition);
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

    #region Projectile Methods

    private void SpawnAngryBird()
    {
        elasticTransform.DOComplete();
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
        cameraManager.SwitchToIdleCam();
    }
    #endregion

    #region Slingshot Animation

    private void AnimateSlingshot()
    {
        elasticTransform.position = leftLineRenderer.GetPosition(0);

        float dist = Vector2.Distance(elasticTransform.position, centerPosition.position);

        float time = dist / elasticDivider;

        elasticTransform.DOMove(centerPosition.position, time).SetEase(elasticCurve);
        StartCoroutine(AnimateSlingshotLines(elasticTransform, time));
    }

    private IEnumerator AnimateSlingshotLines(Transform trans, float time)
    {
        float elapsedTime = 0f;
        while(elapsedTime < time && elapsedTime < maximumAnimationTime)
        {
            elapsedTime += Time.deltaTime;

            SetLines(trans.position);

            yield return null;
        }
    }

    #endregion
}