using UnityEngine;

public class ConcentricCirclesShooter : Shooter
{
    public int rings = 3; // Number of concentric circles
    public int bulletsPerRing = 8; // Number of bullets per ring
    public float distanceBetweenRings = 0.5f; // Distance between each ring

    protected override void Shoot()
    {
        for (int ring = 0; ring < rings; ring++)
        {
            float radius = ring * distanceBetweenRings;
            for (int i = 0; i < bulletsPerRing; i++)
            {
                float angle = i * (360f / bulletsPerRing);
                Quaternion rotation = Quaternion.Euler(0, 0, angle);
                Vector3 spawnPosition = shootPoint.position + (rotation * Vector3.up * radius);
                Instantiate(bulletPrefab, spawnPosition, rotation);
            }
        }
    }
}