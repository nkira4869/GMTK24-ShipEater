using UnityEngine;

public class CircularShotShooter : Shooter
{
    public int bulletCount = 8; // Number of bullets in the circle

    protected override void Shoot()
    {
        float angleStep = 360f / bulletCount;

        for (int i = 0; i < bulletCount; i++)
        {
            Quaternion rotation = shootPoint.rotation * Quaternion.Euler(0, 0, i * angleStep);
            Instantiate(bulletPrefab, shootPoint.position, rotation);
        }
    }
}