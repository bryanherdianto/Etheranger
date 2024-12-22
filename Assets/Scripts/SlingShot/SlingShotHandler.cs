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
    [SerializeField] private TrajectoryLine trajectoryLine;

    [Header("Projectiles")]
    [SerializeField] public Projectile[] projectilePrefabs;
    [SerializeField] private float projectilePositionOffset = 2f;
    [SerializeField] private float timeBetweenProjectileRespawns = 3f;

    [Header("Sound")]
    [SerializeField] private AudioClip slingshotPullSound;
    [SerializeField] private AudioClip slingshotReleaseSound;

    [Header("Bomb Projectile")]
    [SerializeField] private float bombBlastForce = 3f;
    [SerializeField] private float bombBlastRadius = 3f;

    private Vector2 slingShotLinePosition;
    private Vector2 direction;
    private Vector2 directionNormalized;

    private bool clickedWithinArea;
    private bool projectileOnSlingshot;

    private Projectile spawnedProjectile;

    private AudioSource audioSource;

    private int currentProjectileIndex = 0;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        leftLineRenderer.enabled = false;
        rightLineRenderer.enabled = false;

        SpawnProjectile();
    }

    private void Update()
    {
        if (InputManager.wasLeftPressed && slingShotArea.isWithinSlingShotArea())
        {
            clickedWithinArea = true;

            if (projectileOnSlingshot)
            {
                SoundManager.instance.PlayClip(slingshotPullSound, audioSource);
                cameraManager.SwitchToFollowCam(spawnedProjectile.transform);
            }
        }

        if (InputManager.isLeftPressed && clickedWithinArea && projectileOnSlingshot)
        {
            DrawLine();
            PositionAndRotateProjectile();
            trajectoryLine.DrawTrajectory(direction, shotForce);
        }

        if (InputManager.wasLeftReleased && projectileOnSlingshot && clickedWithinArea)
        {
            if(GameManager.instance.HasEnoughProjectiles())
            {
                clickedWithinArea = false;
                projectileOnSlingshot = false;

                GameManager.instance.UseShot();
                
                spawnedProjectile.LaunchProjectile(direction, shotForce);

                trajectoryLine.ClearTrajectory();

                SoundManager.instance.PlayClip(slingshotReleaseSound, audioSource);

                AnimateSlingshot();

                if(GameManager.instance.HasEnoughProjectiles())
                {
                    StartCoroutine(SpawnProjectileAfterTime());
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

    private void SpawnProjectile()
    {
        elasticTransform.DOComplete();
        SetLines(idlePosition.position);

        Vector2 dir = (centerPosition.position - idlePosition.position).normalized;
        Vector2 spawnPosition = (Vector2)idlePosition.position + dir * projectilePositionOffset;
        spawnedProjectile = Instantiate(projectilePrefabs[currentProjectileIndex], spawnPosition, Quaternion.identity);
        spawnedProjectile.transform.right = dir;
        
        currentProjectileIndex = (currentProjectileIndex + 1) % projectilePrefabs.Length;

        switch(spawnedProjectile.projectileType)
        {
            case 0:
                spawnedProjectile.SetProjectileBehavior(new NormalProjectileBehavior());
                break;
            case 1:
                spawnedProjectile.SetProjectileBehavior(new BombProjectileBehavior(bombBlastForce, bombBlastRadius, spawnedProjectile.GetComponent<Animator>()));
                break;
            case 2:
                spawnedProjectile.SetProjectileBehavior(new ShurikenProjectileBehavior(bombBlastForce, bombBlastRadius, spawnedProjectile.GetComponent<Animator>()));
                break;
            default:
                break;
        }

        projectileOnSlingshot = true;
    }

    private void PositionAndRotateProjectile()
    {
        spawnedProjectile.transform.position = slingShotLinePosition + directionNormalized * projectilePositionOffset;
        spawnedProjectile.transform.right = directionNormalized;
    }

    private IEnumerator SpawnProjectileAfterTime()
    {
        yield return new WaitForSeconds(timeBetweenProjectileRespawns);
        SpawnProjectile();
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