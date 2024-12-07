using UnityEngine;

public class Blaster : MonoBehaviour
{
    [Header("Blaster Settings")]
    public Transform weaponBarrel; // Position and direction of the weapon's barrel
    public GameObject bulletPrefab; // Bullet prefab to instantiate
    public float fireRate = 1f; // Time between shots
    private float nextFireTime;
    public Vector3 targetOffset;
    public void FireAtTarget(Transform target)
    {
        ShootBullet(target);

        //if (Time.time >= nextFireTime)
        //{
           
        //    nextFireTime = Time.time + fireRate;
        //}
    }

    private void ShootBullet(Transform target)
    {
        if (bulletPrefab == null || weaponBarrel == null || target == null)
        {
            Debug.LogWarning("Blaster is missing required references.");
            return;
        }

        // Instantiate the bullet
        GameObject bulletInstance = Instantiate(bulletPrefab, weaponBarrel.position, weaponBarrel.rotation);

        // Initialize the bullet to move towards the target
        Bullet bullet = bulletInstance.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.Initialize(target.position + targetOffset);
        }

        Debug.Log("Blaster fired a bullet.");
    }
}
