using UnityEngine;

public class SingleShotShooter : Shooter
{
    protected override void Shoot()
    {
        // Instantiate a single bullet
        Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
    }
}
