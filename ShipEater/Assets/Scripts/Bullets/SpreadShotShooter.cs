using UnityEngine;

public class SpreadShotShooter : Shooter
{
    public int bulletCount = 5; // Number of bullets in the spread
    public float spreadAngle = 45f; // The angle of the spread

    protected override void Shoot()
    {
        float angleStep = spreadAngle / (bulletCount - 1);
        float startAngle = -spreadAngle / 2;

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = startAngle + i * angleStep;
            Quaternion rotation = shootPoint.rotation * Quaternion.Euler(0, 0, angle);
            Instantiate(bulletPrefab, shootPoint.position, rotation);
        }
    }
}
