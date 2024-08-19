using UnityEngine;

public class RandomDirectionShooter : Shooter
{
    public float spreadAngle = 45f; 

    protected override void Shoot()
    {
        float randomAngle = Random.Range(-spreadAngle / 2, spreadAngle / 2);
        Quaternion rotation = shootPoint.rotation * Quaternion.Euler(0, 0, randomAngle);
        Instantiate(bulletPrefab, shootPoint.position, rotation);
    }
}
