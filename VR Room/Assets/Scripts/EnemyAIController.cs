using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAIController : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    private Animator anim;
    private NavMeshAgent agent;

    [Header("AI Settings")]
    public float detectionRange = 5f;
    public float runRange = 15f;
    public float walkRange = 7f;
    public float attackRange = 2.5f;
    public float attackCooldown = 2f;
    public float rotationSpeed = 5f;

    [Header("Health")]
    public float maxHealth = 100f;
    private float currentHealth;
    private bool isDead = false;

    private float lastAttackTime;
    private bool isAttacking = false;
    private bool inAttackAnim = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        currentHealth = maxHealth;

        // Auto-find XR player rig
        if (player == null)
        {
            var rig = GameObject.FindObjectOfType<Unity.XR.CoreUtils.XROrigin>();
            if (rig != null) player = rig.Camera.transform;
            else
            {
                GameObject p = GameObject.FindGameObjectWithTag("Player");
                if (p != null) player = p.transform;
            }
        }
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        Debug.Log(distance);
        if (distance > detectionRange)
        {
            Debug.Log("IDLE");
            Idle();
        }
        else if (distance < detectionRange && distance > attackRange)
        {
            Debug.Log("MOVE TO PLAYER");
            MoveTowardsPlayer(distance);
        }
        else
        {
            Debug.Log("ATTACK PLAYER");
            AttackPlayer();
        }
    }

    private void MoveTowardsPlayer(float distance)
    {
        if (agent == null || inAttackAnim) return;

        agent.isStopped = false;
        agent.SetDestination(player.position);

        // Reset animation states
        anim.ResetTrigger("isAttacking");
        anim.SetBool("isIdle", false);
        anim.SetBool("isWalking", false);
        anim.SetBool("isRunning", false);

        // Walk / Run logic
        if (distance > runRange)
        {
            anim.SetBool("isRunning", true);
            agent.speed = 4.5f;
        }
        else
        {
            anim.SetBool("isWalking", true);
            agent.speed = 2f;
        }
    }

    private void Idle()
    {
        if (agent == null) return;

        agent.isStopped = true;
        anim.SetBool("isIdle", true);
        anim.SetBool("isWalking", false);
        anim.SetBool("isRunning", false);
    }

    private void AttackPlayer()
    {
        if (isDead) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Always face the player
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

        // If too far from target, move closer instead of attacking
        if (distance > attackRange + 0.2f)
        {
            // Cancel attack state if currently in one
            if (inAttackAnim)
            {
                isAttacking = false;
                inAttackAnim = false;
            }

            // Resume movement toward player
            agent.isStopped = false;
            MoveTowardsPlayer(distance);
            return;
        }

        // Attack only when in range and cooldown ready
        if (!isAttacking && !inAttackAnim && Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            isAttacking = true;
            inAttackAnim = true;

            agent.isStopped = true; // Stop to attack
            agent.velocity = Vector3.zero;

            anim.SetTrigger("isAttacking");
            anim.SetBool("isIdle", false);
            anim.SetBool("isWalking", false);
            anim.SetBool("isRunning", false);

            StartCoroutine(EndAttackAfterDelay(1.2f)); // match your animation duration
        }
    }

    private IEnumerator EndAttackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        isAttacking = false;
        inAttackAnim = false;

        float distance = Vector3.Distance(transform.position, player.position);

        // If player ran away, chase immediately
        if (distance > attackRange)
        {
            agent.isStopped = false;
            MoveTowardsPlayer(distance);
        }
        else
        {
            // Still close stay ready to attack again
            agent.isStopped = false;
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        anim.SetTrigger("isHit");

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        isDead = true;
        agent.isStopped = true;

        anim.SetTrigger("Die");
        anim.SetBool("isIdle", false);
        anim.SetBool("isWalking", false);
        anim.SetBool("isRunning", false);

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Destroy(gameObject, 5f);
    }
}
