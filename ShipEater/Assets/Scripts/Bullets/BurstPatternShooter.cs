using System.Collections;
using UnityEngine;

public class BurstPatternShooter : Shooter
{
    public int bulletsPerBurst = 5; // Number of bullets per burst
    public float burstRate = 0.1f; // Time between bullets in a burst

    protected override void Shoot()
    {
        StartCoroutine(FireBurst());
    }

    private IEnumerator FireBurst()
    {
        for (int i = 0; i < bulletsPerBurst; i++)
        {
            Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
            yield return new WaitForSeconds(burstRate); // Wait for a short interval before firing the next bullet
        }
    }
}