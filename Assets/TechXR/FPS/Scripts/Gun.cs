using System;
using System.Collections;
using System.Collections.Generic;
using TechXR.Core.Sense;
using UnityEngine;

public class Gun : MonoBehaviour, IWeapon
{
    // Public fields
    public GameObject BulletPrefab;
    public Transform BulletSpawnPoint;
    public float Damage => this.damage;
    public float Range = 100f;
    public float ReloadTime = 1f;
    //public float bulletForce = 40000000000.0f;
    public float bulletForce = Mathf.Pow(10, 100);
    public GameObject MuzzleEffect;

    // Private fields
    [SerializeField] private AudioClip ShootSound;
    [SerializeField] private AudioClip ReloadSound;
    [SerializeField] private float damage = 15f;
    private float m_currentAmmo;
    private float maxAmmo = 10;
    private AudioSource _audioSource;
    private ParticleSystem m_MuzzleEffect;

    // Internal fields
    bool isReloading = false;

    private void OnEnable()
    {
        isReloading = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        m_currentAmmo = maxAmmo;

        m_MuzzleEffect = MuzzleEffect.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        // If the Gun is in reloading mode then return
        if (isReloading) return;
        // Check if no ammo then auto-reload
        if (m_currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }
        // Call the Shoot function after recieving the input
        if (SenseInput.GetButtonDown(ButtonName.U))
        {
            Debug.Log("Shooting A Bullet");
            if (!isReloading) Shoot();
        }
    }

    // IWeapon - Shoot()
    public void Shoot()
    {
        // Decrement of one ammo after each call
        if (m_currentAmmo > 0)
            m_currentAmmo--;
        else
            return;

        // Play Muzzle Effect particle system
        m_MuzzleEffect.Play();

        // Play the shoot audio
        _audioSource.clip = ShootSound;
        _audioSource.Play();

        // Instantiate the bullet and give the velocity
        GameObject bulletObject = ObjectPoolingManager.Instance.GetBullet();
        bulletObject.transform.position = BulletSpawnPoint.position;
        bulletObject.transform.forward = BulletSpawnPoint.forward;
        bulletObject.GetComponent<Rigidbody>().velocity = bulletObject.transform.forward * bulletForce;
    }

    // IWeapon - Reload()
    public IEnumerator Reload()
    {
        // Make Reloading mode onn
        isReloading = true;

        // Play the reload audio
        _audioSource.clip = ReloadSound;
        _audioSource.Play();

        yield return new WaitForSeconds(ReloadTime);

        // Load the ammo
        m_currentAmmo = maxAmmo;

        // Off Reloading mode
        isReloading = false;
    }
}