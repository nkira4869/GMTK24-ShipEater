using UnityEngine;

public class WavePatternShooter : Shooter
{
    public float waveFrequency = 2f; // How often the bullet moves up and down
    public float waveAmplitude = 1f; // How far the bullet moves up and down
    public int bulletCount = 5; // Number of bullets per wave

    protected override void Shoot()
    {
        for (int i = 0; i < bulletCount; i++)
        {
            // Instantiate the bullet and add a wave behavior
            GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
            WaveBullet waveBullet = bullet.AddComponent<WaveBullet>();
            waveBullet.SetWaveProperties(waveFrequency, waveAmplitude);
        }
    }
}

public class WaveBullet : MonoBehaviour
{
    public float speed = 5f; // Bullet speed
    private float frequency;
    private float amplitude;
    private float timeSinceSpawn;

    public void SetWaveProperties(float freq, float amp)
    {
        frequency = freq;
        amplitude = amp;
    }

    void Update()
    {
        timeSinceSpawn += Time.deltaTime;
        // Move the bullet forward
        transform.Translate(Vector3.up * speed * Time.deltaTime);
        // Add a sinusoidal wave movement along the x-axis
        transform.position += transform.right * Mathf.Sin(timeSinceSpawn * frequency) * amplitude * Time.deltaTime;
    }
}