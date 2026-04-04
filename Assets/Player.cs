using Unity.Mathematics;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private BallSpawner ballSpawner;

    [SerializeField] public int initPlayerHealth = 1;
    public int playerHealth = 1;

    [SerializeField] private AudioClip shootSound;

    private float shootTimer;

    float mouseX = 0;

    public void ResetPlayer()
    {
        playerHealth = initPlayerHealth;
        mouseX = 0;
    }

    void Update()
    {
        shootTimer += Time.deltaTime;

        if (Input.GetKey(KeyCode.Mouse0))
        {
            mouseX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
            mouseX = math.clamp(mouseX, -6.5f, 6.5f);

            if (shootTimer >= GameManager.Instance.fireRate)
            {
                Shoot();
                shootTimer = 0f;
            }
        }

        Vector3 pos = transform.position;
        pos.x = Mathf.Lerp(pos.x, mouseX, 0.2f);
        pos.y = -6.5f;
        transform.position = pos;
    }

    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().Initialize(
            GameManager.Instance.bulletDamage,
            GameManager.Instance.lightningDamage,
            GameManager.Instance.lightningBounces,
            GameManager.Instance.poisonDamagePerSec,
            GameManager.Instance.poisonDuration,
            ballSpawner
        );

        AudioSource.PlayClipAtPoint(shootSound, Camera.main.transform.position, 1.0f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<Ball>(out Ball ball)) return;
        {
            Debug.Log("Pdamage");
            playerHealth -= 1;
        }
    }
}