using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour, IDamageable
{
    // Player Stats
    public float health = 500;
    public float maxHealth = 500;
    public float movementSpeed = 2.5f;
    public float attacksPerSecond = 1.0f;
    public float attacksPerSecondFire = 0.2f;
    public float attackRangeFire = 4.0f;
    public float attackDamageFire = 50.0f;
    public float attackRange = 2.0f;
    public float attackDamage = 40.0f;
    [HideInInspector] public bool isDead;

    // Visual Effects
    public GameObject slashEffect;
    public GameObject fireEffect;

    // Variables to control when the unit can attack and move.
    private float canCastAt;
    private float canMoveAt;

    //movement
    public bool isStopped;
    public bool froze;

    // Constants to prevent magic numbers in the code. Makes it easier to edit later.
    private const float MOVEMENT_DELAY_AFTER_CASTING = 0.2f;
    private const float MOVEMENT_DELAY_AFTER_CASTING_FIRE = 0.75f;
    private const float TURNING_SPEED = 15.0f;

    // Cache references to important components for easy access later.
    private NavMeshAgent agentNavigation;
    private Animator animator;

    // Variables to control ability casting.
    private enum Ability { Cleave, Fire/* Add more abilities in here! */ }
    private Ability? abilityBeingCast = null;
    private float finishAbilityCastAt;
    private Vector3 abilityTargetLocation;

    public GameOver script1;
    public UnitUI UIScript;
    public MonsterSpawner MonsterSpawnerScript;

    private void Start()
    {
        agentNavigation = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        StartCoroutine(PlayingAnimationAfterSpawning());
    }

    private void Update()
    {
        if (isDead)
        {
            gameObject.SetActive(false);
            return;
        }
        UpdateMovement();
        UpdateAbilityCasting();
    }

    IEnumerator PlayingAnimationAfterSpawning()
    {
        animator.Play("Crouch To Stand");
        agentNavigation.isStopped = true;
        yield return new WaitForSeconds(5);
        agentNavigation.isStopped = false;
    }

    // Handle all update logic associated with the character's movement.
    private void UpdateMovement()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (UIScript.Stamina > 0)
            {
                agentNavigation.speed = 5f; 
                animator.SetFloat("Speed", agentNavigation.velocity.magnitude);
                UIScript.RunStaminaDrain();
            }
            else
            {
                agentNavigation.speed = 2.5f; 
                animator.SetFloat("Speed", agentNavigation.velocity.magnitude);
            }
        }
        else
        {
            agentNavigation.speed = 2.5f;
            animator.SetFloat("Speed", agentNavigation.velocity.magnitude);
            UIScript.RechargeStamina();
        }

        if (Input.GetMouseButton(0) && Time.time > canMoveAt) 
        { 
            agentNavigation.SetDestination(Utilities.GetMouseWorldPosition());
            UIScript.RechargeStamina();
        }
        else
        {
            agentNavigation.SetDestination(transform.position);
            UIScript.RechargeStamina();
        }
    }

    // Handle all update logic associated with ability casting.
    private void UpdateAbilityCasting()
    {
        // If the right click button is held and the player can cast, start a basic attack cast.
        if (Input.GetMouseButton(1) && Time.time > canCastAt)
            StartCastingCleave();
        
        if(Input.GetKeyDown(KeyCode.E) && Time.time > canCastAt)
            if (UIScript.Stamina > 50)
            {
                StartCastingFire();
            }

        // If the current ability has reached the end of its cast, run the appropriate actions for the ability.
        if (abilityBeingCast != null && Time.time > finishAbilityCastAt)
        {
            switch (abilityBeingCast)
            {
                case Ability.Cleave:
                    FinishCastingCleave();
                    break;

                case Ability.Fire:
                    if (UIScript.Stamina > 50)
                    {
                        FinishCastingFire();
                    }
                    break;
            };
        }

        // If a cast is in progress, have the player face towards the target location.
        if (abilityBeingCast != null && abilityBeingCast == Ability.Cleave)
        {
            Quaternion look = Quaternion.LookRotation((abilityTargetLocation - transform.position).normalized);
            transform.rotation = Quaternion.Lerp(transform.rotation, look, Time.deltaTime * TURNING_SPEED);
        }
    }

    // Perform all logic for when the player *starts* casting the cleave ability.
    private void StartCastingCleave()
    {
        // Stop the character from moving while they attack.
        agentNavigation.SetDestination(transform.position);

        // Set the ability being cast to the cleave ability.
        abilityBeingCast = Ability.Cleave;

        // Play the appropriate ability animation at the correct speed.
        animator.CrossFadeInFixedTime("Attack", 0.2f);
        animator.SetFloat("AttackSpeed", attacksPerSecond);

        // Calculate when the ability will finish casting, and when the player can next cast and move.
        float castTime = (1.0f / attacksPerSecond);
        canCastAt = Time.time + castTime;
        finishAbilityCastAt = Time.time + 0.4f * castTime;
        canMoveAt = finishAbilityCastAt + MOVEMENT_DELAY_AFTER_CASTING;
        abilityTargetLocation = Utilities.GetMouseWorldPosition();
    }

    // Perform all logic for when the player *finishes* casting the cleave ability.
    private void FinishCastingCleave()
    {
        // Clear the ability currently being cast.
        abilityBeingCast = null;

        // Create the slash visual and destroy it after it plays.
        if (slashEffect != null)
        {
            GameObject slashVisual = Instantiate(slashEffect, transform.position, transform.rotation);
            Destroy(slashVisual, 1.0f);
        }

        // Find all the targets that should be hit by the attack and damage them.
        Vector3 hitPoint = transform.position + transform.forward * attackRange;
        List<Monster> targets = Utilities.GetAllWithinRange<Monster>(hitPoint, attackRange);
        foreach (Monster target in targets)
            target.TakeDamage(attackDamage);

        List<MonsterSpawner> targets2 = Utilities.GetAllWithinRange<MonsterSpawner>(hitPoint, attackRange);
        foreach (MonsterSpawner target in targets2)
            target.TakeDamage(attackDamage);


        UIScript.AttackStaminaDrain();
    }

    private void StartCastingFire()
    {
        // Stop the character from moving while they attack.
        agentNavigation.SetDestination(transform.position);

        // Set the ability being cast to the cleave ability.
        abilityBeingCast = Ability.Fire;

        // Play the appropriate ability animation at the correct speed.
        animator.CrossFadeInFixedTime("Fire Attack", 2.0f);
        animator.SetFloat("AttackSpeedFire", attacksPerSecondFire);

        // Calculate when the ability will finish casting, and when the player can next cast and move.
        float castTime = 1.5f / attacksPerSecondFire;
        canCastAt = Time.time + 4.5f;
        finishAbilityCastAt = Time.time + 0.4f * castTime;
        canMoveAt = finishAbilityCastAt + MOVEMENT_DELAY_AFTER_CASTING_FIRE;
    }

    private void FinishCastingFire()
    {
        // Clear the ability currently being cast.
        abilityBeingCast = null;

        // Create the slash visual and destroy it after it plays.
        if (fireEffect != null)
        {
            GameObject fireVisual = Instantiate(fireEffect, transform.position, transform.rotation);
            Destroy(fireVisual, 1.0f);
        }

        // Find all the targets that should be hit by the attack and damage them.
        Vector3 hitPoint = transform.position;
        List<Monster> targets = Utilities.GetAllWithinRange<Monster>(hitPoint, attackRangeFire);
        foreach (Monster target in targets)
            target.TakeDamage(attackDamageFire);

        List<MonsterSpawner> targets2 = Utilities.GetAllWithinRange<MonsterSpawner>(hitPoint, attackRangeFire);
        foreach (MonsterSpawner target in targets2)
            target.TakeDamage(attackDamageFire);
            MonsterSpawnerScript.freezeBool = true;


        UIScript.FireAttackStaminaDrain();
    }

    // Remove the specified amount of health from this unit, killing it if needed.
    public virtual void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
            Kill();
    }

    // Destroy the player, but briefly keeping the corpse visible to play the death animation.
    public virtual void Kill()
    {
        isDead = true;
        agentNavigation.SetDestination(transform.position);
        script1.isGameOver = true;
    }

    IEnumerator wait()
    {
        yield return new WaitForSeconds(3);
    }

    // Returns the current health percent of the character (a value between 0.0 and 1.0).
    public float GetCurrentHealthPercent()
    {
        return health / maxHealth;
    }

}
