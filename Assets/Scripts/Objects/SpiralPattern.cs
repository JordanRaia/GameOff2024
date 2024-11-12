using System.Collections;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "SpiralPattern", menuName = "BulletHell/SpiralPattern")]
public class SpiralPattern : BulletHellPattern
{
    public GameObject bulletPrefab;
    public float bulletSpeed = 5f;
    public int bulletsPerWave = 10;
    public int totalWaves = 5;
    public float timeBetweenWaves = 0.5f;
    public float angleOffsetPerWave = 10f; // New variable for rotation
    public float spawnRadius = 5f; // Radius around the player to spawn bullets

    private Vector2 fixedTargetPosition; // Store the fixed target position if static

    public override IEnumerator ExecutePattern(Func<Vector2> getTargetPosition)
    {
        int wave = 0;

        while (true)
        {
            Vector2 currentTargetPosition = isStatic ? fixedTargetPosition : getTargetPosition();

            for (int i = 0; i < bulletsPerWave; i++)
            {
                float angle = (i * (360f / bulletsPerWave)) + (wave * angleOffsetPerWave);
                SpawnBullet(angle, currentTargetPosition);
            }

            wave++;
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    void SpawnBullet(float angle, Vector2 targetPosition) // Changed to Vector2
    {
        // Calculate spawn position around the target position at spawnRadius distance
        Vector2 offset = new Vector2(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad)
        ) * spawnRadius;

        Vector2 spawnPosition = targetPosition + offset;

        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        // Set bullet to move towards the target position
        Vector2 direction = (targetPosition - spawnPosition).normalized;
        bullet.GetComponent<Rigidbody2D>().linearVelocity = direction * bulletSpeed;
        activeBullets.Add(bullet);
    }

    // Optional: Initialize fixedTargetPosition when the pattern starts if static
    public void Initialize(Vector2 initialPosition)
    {
        if (isStatic)
        {
            fixedTargetPosition = initialPosition;
        }
    }
}