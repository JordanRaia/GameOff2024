using System.Collections;
using UnityEngine;

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

    public override IEnumerator ExecutePattern(GameObject player)
    {
        int wave = 0;
        while (true)
        {
            for (int i = 0; i < bulletsPerWave; i++)
            {
                float angle = (i * (360f / bulletsPerWave)) + (wave * angleOffsetPerWave);
                SpawnBullet(angle, player.transform.position);
            }
            wave++;
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    void SpawnBullet(float angle, Vector2 playerPosition)
    {
        // Calculate spawn position around the player at spawnRadius distance
        Vector2 offset = new Vector2(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad)
        ) * spawnRadius;

        Vector2 spawnPosition = playerPosition + offset;

        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        // Set bullet to move towards the player
        Vector2 direction = (playerPosition - spawnPosition).normalized;
        bullet.GetComponent<Rigidbody2D>().linearVelocity = direction * bulletSpeed;
        activeBullets.Add(bullet);
    }
}