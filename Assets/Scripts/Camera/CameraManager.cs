using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera idleCam;
    [SerializeField] private CinemachineVirtualCamera followCam;
    [SerializeField] private float moveDuration = 2f;

    private Vector3 originalPosition;

    private void Awake()
    {
        originalPosition = idleCam.transform.position;
        StartCoroutine(MoveIdleCamToRightmostEnemy());
        SwitchToIdleCam();
    }

    private IEnumerator MoveIdleCamToRightmostEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0)
        {
            yield break;
        }

        // Find the rightmost enemy
        Transform rightmostEnemy = enemies[0].transform;
        foreach (var enemy in enemies)
        {
            if (enemy.transform.position.x > rightmostEnemy.position.x)
            {
                rightmostEnemy = enemy.transform;
            }
        }

        // Move the idleCam to the rightmost enemy
        Vector3 targetPosition = new Vector3(rightmostEnemy.position.x, originalPosition.y, originalPosition.z);
        idleCam.transform.position = targetPosition;

        yield return new WaitForSeconds(2f); // Pause for 2 seconds at the beginning of the game

        float elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            idleCam.transform.position = Vector3.Lerp(targetPosition, originalPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        idleCam.transform.position = originalPosition;
    }

    public void SwitchToIdleCam()
    {
        idleCam.enabled = true;
        followCam.enabled = false;
    }

    public void SwitchToFollowCam(Transform trans)
    {
        followCam.Follow = trans;
        idleCam.enabled = false;
        followCam.enabled = true;
    }
}