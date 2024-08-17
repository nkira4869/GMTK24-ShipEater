using UnityEngine;

public abstract class Shooter : MonoBehaviour
{
    public GameObject bulletPrefab; // Reference to the bullet prefab
    public float fireRate = 1f; // Time between shots
    public Transform shootPoint; // The position where bullets are spawned

    private float nextFireTime;

    void Update()
    {
        if (Time.time >= nextFireTime)
        {
            Shoot(); // Call the Shoot method (implemented in derived classes)
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    // Abstract method for shooting logic, implemented by derived classes
    protected abstract void Shoot();
}