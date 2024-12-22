using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryLine : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private Projectile bulletBehaviour;

    [Header("Trajectory Line Smoothness and Length")]
    [SerializeField] private int lineSegmentCount = 50;
    [SerializeField] private float curveLength = 3.5f;

    private Vector2[] lineSegments;
    private LineRenderer lineRenderer;

    private Rigidbody2D rb;
    private const float TIME_CURVE_ADDITION = 0.5f;

    private void Start()
    {
        lineSegments = new Vector2[lineSegmentCount];
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = lineSegmentCount;
        rb = bulletBehaviour.GetComponent<Rigidbody2D>();
    }

    public void DrawTrajectory(Vector2 direction, float shotForce)
    {
        // Clear trajectory if no force
        if (shotForce <= 0f)
        {
            lineRenderer.enabled = false;
            return;
        }

        lineRenderer.enabled = true;

        Vector2 startPos = bulletSpawnPoint.position;
        lineSegments[0] = startPos;
        lineRenderer.SetPosition(0, startPos);

        // Calculate initial velocity
        Vector2 startVelocity = direction * (shotForce / rb.mass);

        // Populate trajectory line
        for (int i = 1; i < lineSegmentCount; i++)
        {
            float timeOffset = i * Time.fixedDeltaTime * curveLength;

            // Gravity offset based on time
            Vector2 gravityOffset = Physics2D.gravity * Mathf.Pow(timeOffset, 2) * rb.gravityScale * 0.5f;

            // Next point on the trajectory
            lineSegments[i] = lineSegments[0] + startVelocity * timeOffset + gravityOffset;
            lineRenderer.SetPosition(i, lineSegments[i]);
        }
    }

    public void ClearTrajectory()
    {
        lineRenderer.enabled = false;
    }
}
