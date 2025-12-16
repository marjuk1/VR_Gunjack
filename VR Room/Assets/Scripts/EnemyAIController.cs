using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAIController : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    private Animator anim;
    public NavMeshAgent agent;

    [Header("AI Settings")]
    public float detectionRange = 5f;
    public float runRange = 15f;
    public float walkRange = 7f;
    public float attackRange = 2.5f;
    public float attackCooldown = 2f;
    public float rotationSpeed = 5f;

    public float attackDamage = 20f;
    private HealthManager playerHealth;

    [Header("Health")]
    public float maxHealth = 100f;
    private float currentHealth;
    private bool isDead = false;

    private float lastAttackTime;
    private bool isAttacking = false;
    private bool inAttackAnim = false;

    [Header("Item spawning")]
    public GameObject[] lootItems;
    public float SpawnRate = 0.1f;
    [Header("Scoring")]
    public ScoreManager Scoremanager;
    [Header("Footsteps")]
    public AudioSource footstepSource;
    public AudioClip[] footstepClips;
    public float footstepVolume = 0.7f;

    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        if(anim == null)
            anim = GetComponentInChildren<Animator>();
        currentHealth = maxHealth;

        if (footstepSource == null)
            footstepSource = GetComponent<AudioSource>();


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
        playerHealth = FindObjectOfType<HealthManager>();
    }


    void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > detectionRange)
        {
            Idle();
            return;
        }

        if (distance > attackRange)
        {
            MoveTowardsPlayer(distance);
            return;
        }

        TryAttack();
    }

    private void MoveTowardsPlayer(float distance)
    {
        if (agent == null || inAttackAnim && isAttacking) return;

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

    private void TryAttack()
    {
        if (isDead) return;

        // Face player
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(dir),
            Time.deltaTime * rotationSpeed
        );

        // Cooldown check
        if (Time.time - lastAttackTime < attackCooldown) return;

        lastAttackTime = Time.time;

        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        anim.SetTrigger("isAttacking");
    }
    public void EndAttack()
    {
        if (isDead) return;

        agent.isStopped = false;
    }
    // IEnumerator FinishAttack()
    // {
    //     // Deal damage at a consistent time
    //     yield return new WaitForSeconds(0.35f);

    //     // if (playerHealth != null)
    //     //     playerHealth.TakeDamage(attackDamage);

    //     // Reset state
    //     yield return new WaitForSeconds(0.5f);
    //     agent.isStopped = false;
    // }
    public void DealDamage()
    {
        if (isDead || playerHealth == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange + 0.3f)
        {
            playerHealth.TakeDamage(attackDamage);
            Debug.Log("Player hit by enemy attack");
        }
        playerHealth.PlayHurtSound();
    }

    // private IEnumerator EndAttackAfterDelay(float delay)
    // {
    //     yield return new WaitForSeconds(delay);

    //     isAttacking = false;
    //     inAttackAnim = false;

    //     float distance = Vector3.Distance(transform.position, player.position);

    //     // If player ran away, chase immediately
    //     if (distance > attackRange)
    //     {
    //         agent.isStopped = false;
    //         MoveTowardsPlayer(distance);
    //     }
    //     else
    //     {
    //         // Still close stay ready to attack again
    //         agent.isStopped = false;
    //     }
    // }

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
        SpawnLoot();
        Scoremanager.AddScoreOnKill();


        Destroy(gameObject, GetAnimationLength("death1"));
    }
    private float GetAnimationLength(string clipName)
    {
        foreach (AnimationClip clip in anim.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
                return clip.length;
        }
        return 0f;
    }
    private void SpawnLoot()
    {
        if (lootItems.Length == 0) return;

        if (Random.value <= SpawnRate)
        {
            int index = Random.Range(0, lootItems.Length);
            GameObject lootPrefab = lootItems[index];

            Vector3 offset = new Vector3(Random.Range(-0.5f, 0.5f), 0.1f, Random.Range(-0.5f, 0.5f));
            GameObject spawnedLoot = Instantiate(lootPrefab, transform.position + offset, Quaternion.identity);

            // Add AmmoTypeIdentifier dynamically
            AmmoTypeIdentifier identifier = spawnedLoot.AddComponent<AmmoTypeIdentifier>();
            AmmoBeltHelper helper = spawnedLoot.AddComponent<AmmoBeltHelper>();

            // Example: determine type based on prefab name or array index
            if (lootPrefab.name.Contains("Pistol"))
                identifier.type = AmmoType.Pistol;
            else if (lootPrefab.name.Contains("AKM"))
                identifier.type = AmmoType.AKM;
            else if(lootPrefab.name.Contains("RGD"))
            identifier.type = AmmoType.Granade;

            // Ensure XR grab is enabled if needed
            var grab = spawnedLoot.GetComponent<UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable>();
            if (grab != null) grab.enabled = true;

            // Ensure Rigidbody is non-kinematic
            Rigidbody rb = spawnedLoot.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = false;
        }
    }
    public void PlayFootstep()
    {
        if (isDead || footstepClips.Length == 0 || footstepSource == null)
            return;

        AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
        footstepSource.PlayOneShot(clip, footstepVolume);
    }
    public void SetCurrentHealth(float value)
    {
        currentHealth = value;
    }
}
