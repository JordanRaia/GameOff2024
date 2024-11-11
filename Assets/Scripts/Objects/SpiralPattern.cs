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

    public override IEnumerator ExecutePattern(GameObject player)
    {
        for (int wave = 0; wave < totalWaves; wave++)
        {
            for (int i = 0; i < bulletsPerWave; i++)
            {
                float angle = i * (360f / bulletsPerWave);
                SpawnBullet(angle, player.transform.position);
            }
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    void SpawnBullet(float angle, Vector2 centerPosition)
    {
        GameObject bullet = Instantiate(bulletPrefab);
        bullet.transform.position = centerPosition;
        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);
        bullet.GetComponent<Rigidbody2D>().linearVelocity = bullet.transform.up * bulletSpeed;
    }
}