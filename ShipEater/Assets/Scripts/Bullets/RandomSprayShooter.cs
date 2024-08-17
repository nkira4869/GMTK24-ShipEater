using UnityEngine;

public class RandomSprayShooter : Shooter
{
    public float sprayAngle = 45f; // The spread angle of the spray
    public int bulletCount = 10; // Number of bullets in the spray

    protected override void Shoot()
    {
        for (int i = 0; i < bulletCount; i++)
        {
            float randomAngle = Random.Range(-sprayAngle / 2, sprayAngle / 2);
            Quaternion rotation = shootPoint.rotation * Quaternion.Euler(0, 0, randomAngle);
            Instantiate(bulletPrefab, shootPoint.position, rotation);
        }
    }
}