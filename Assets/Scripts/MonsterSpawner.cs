using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class MonsterSpawner : MonoBehaviour, IDamageable
{
    public float timeBetweenSpawns = 2.0f;
    public float spawnRadius = 10.0f;
    public Monster monsterToSpawn;
    public GameObject monsterSpawnEffect;
    

    //new settings
    public float health = 2000f;
    public float maxHealth = 2000f;
    private const float TIME_BEFORE_CORPSE_DESTROYED = 0.5f; 
    private Animator animator;
    private ParticleSystem explosionEffect;
    


    private float nextSpawnAt;
    private Player player;

    void Start()
    {
        player = GameObject.FindObjectOfType<Player>();
        animator = GetComponentInChildren<Animator>();
        explosionEffect = GameObject.Find("Explosion").GetComponent<ParticleSystem>();

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

            // Spawn the monster at the correct spawn location (and make its own game object)
            GameObject clones = Instantiate(monsterToSpawn.gameObject, spawnPosition, transform.rotation);

            //Created clones are children of the monster spawner
            clones.transform.parent = this.transform;


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

    public virtual void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
            Kill();
    }

    public virtual void Kill()
    {
        foreach (Transform child in this.transform)
        {
            monsterToSpawn.Kill();
            Debug.Log("dead!");
        }
        explosionEffect.Play();
        Destroy(gameObject, TIME_BEFORE_CORPSE_DESTROYED);
    }

    public float GetCurrentHealthPercent()
    {
        return health / maxHealth;
    }


}
