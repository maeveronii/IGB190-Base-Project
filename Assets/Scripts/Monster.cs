using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    // Player Stats
    public float health = 40;
    public float maxHealth = 40;
    public float movementSpeed = 1.0f;
    public float attacksPerSecond = 1.0f;
    public float attackRange = 2.0f;
    public float attackDamage = 10.0f;

    // Cache references to important components for easy access later.
    private NavMeshAgent agentNavigation;
    private Animator animator;
    private Player player;

    void Start()
    {
        agentNavigation = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        player = GameObject.FindObjectOfType<Player>();
    }

    private void Update()
    {
        UpdateMovement();
    }

    // Handle all update logic associated with the character's movement.
    private void UpdateMovement()
    {
        animator.SetFloat("Speed", agentNavigation.velocity.magnitude);
        if (Input.GetMouseButton(0))
            agentNavigation.SetDestination(player.transform.position);
    }
}
