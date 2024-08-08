using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public float timeBetweenSpawns = 2.0f;
    public float spawnRadius = 10.0f;
    public Monster monsterToSpawn;
    public GameObject monsterSpawnEffect;
    private float nextSpawnAt;
    private Player player;

    void Start()
    {
        player = GameObject.FindObjectOfType<Player>();
    }


    void Update()
    {
        if (monsterToSpawn != null && Time.time > nextSpawnAt)
        {
            // Calculate the correct spawn location (given the set spawn radius).
            Vector3 spawnPosition = transform.position + Random.insideUnitSphere * spawnRadius;
            spawnPosition.y = transform.position.y;

            // Calculate when the next monster should be spawned.
            nextSpawnAt = Time.time + timeBetweenSpawns;

            // Spawn the monster at the correct spawn location.
            Instantiate(monsterToSpawn.gameObject, spawnPosition, transform.rotation);

            // If a spawn effect has been assigned, spawn it.
            if (monsterSpawnEffect != null)
                Instantiate(monsterSpawnEffect, spawnPosition, Quaternion.identity);
        }
        if (player.isDead)
        {
            gameObject.SetActive(false);
            return;
        }
    }
}
