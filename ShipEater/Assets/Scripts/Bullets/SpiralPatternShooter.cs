using UnityEngine;

public class SpiralPatternShooter : Shooter
{
    public int bulletCount = 10; // Number of bullets per wave
    public float rotationSpeed = 60f; // Speed at which the spiral rotates
    private float currentAngle = 0f;

    protected override void Shoot()
    {
        for (int i = 0; i < bulletCount; i++)
        {
            float angle = currentAngle + (360f / bulletCount) * i;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation * rotation);
        }

        // Increase the current angle to create a continuous spiral
        currentAngle += rotationSpeed * Time.deltaTime;
    }
}