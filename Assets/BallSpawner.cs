using UnityEngine;
using System.Collections.Generic;

public class BallSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private int spawnLimit = 0;
    [SerializeField] private int healthMultiplier = 1;
    [SerializeField] private bool spawnOnStart = true;
    [SerializeField] private BallType defaultBallType = BallType.Dodecagon;

    [Header("Spawn Area")]
    [SerializeField] private Vector2 spawnOffset = Vector2.zero;
    [SerializeField] private float spawnRadius = 0f;

    [Header("Ball Appearance")]
    [SerializeField] private int resolution = 64;

    [Header("Physics")]
    [SerializeField] private float gravity = 9.8f;
    [SerializeField] private float xLim = 10f;
    [SerializeField] private float floorHeight = -5f;

    [SerializeField] private AudioClip[] ballSounds;

    private float timer;
    private bool isSpawning;
    public List<Ball> balls = new List<Ball>();

    public List<CurrencyDrop> currencyDrops = new List<CurrencyDrop>();

    void Start()
    {
        if (spawnOnStart) StartSpawning();
    }

    void Update() { // gameplay loop goes here
        if (isSpawning) {
            if (balls.Count <= spawnLimit) {
                timer += Time.deltaTime;

                if (timer >= spawnInterval) {
                    SpawnBall(BallType.Pentagon);
                    SpawnBall(BallType.Octagon);
                    timer = 0f;
                }
            } else {
                timer = 0f;
            }
        }

        foreach (Ball ball in balls)
            ball.UpdatePhysics(Time.deltaTime, gravity, xLim, floorHeight);

        foreach (CurrencyDrop drop in currencyDrops)
            drop.UpdatePhysics(Time.deltaTime, gravity, xLim, floorHeight);
    }

    public GameObject SpawnBall(BallType type)
    {
        Vector2 spawnPos = (Vector2)transform.position + spawnOffset;
        if (spawnRadius > 0)
            spawnPos += Random.insideUnitCircle * spawnRadius;

        GameObject ballObj = new GameObject($"Ball_{type}");
        ballObj.transform.position = spawnPos;

        Ball ball = ballObj.AddComponent<Ball>();
        ball.Initialize(type, resolution, this, healthMultiplier, ballSounds);

        balls.Add(ball);
        return ballObj;
    }

    public void RemoveBall(Ball ball)
    {
        balls.Remove(ball);
    }

    public GameObject SpawnCurrencyDrop(Vector3 position, Vector2 velocity)
    {
        GameObject dropObj = new GameObject("CurrencyDrop");
        dropObj.transform.position = position;

        CurrencyDrop drop = dropObj.AddComponent<CurrencyDrop>();
        drop.Initialize(velocity);

        currencyDrops.Add(drop);
        return dropObj;
    }

    public void RemoveCurrencyDrop(CurrencyDrop drop)
    {
        currencyDrops.Remove(drop);
    }

    public void StartSpawning() { isSpawning = true; timer = 0f; }
    public void StopSpawning() { isSpawning = false; }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector2 pos = (Vector2)transform.position + spawnOffset;
        Gizmos.DrawWireSphere(pos, 0.2f);
        if (spawnRadius > 0)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(pos, spawnRadius);
        }
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(new Vector3(-xLim, floorHeight - 2, 0), new Vector3(-xLim, floorHeight + 10, 0));
        Gizmos.DrawLine(new Vector3(xLim, floorHeight - 2, 0), new Vector3(xLim, floorHeight + 10, 0));
        Gizmos.DrawLine(new Vector3(-xLim - 2, floorHeight, 0), new Vector3(xLim + 2, floorHeight, 0));
    }
}