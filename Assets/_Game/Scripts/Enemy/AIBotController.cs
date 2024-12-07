using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;
public class AIBotController : MonoBehaviour
{
    public enum BotType
    {
        Drone,
        Enforcer,
        Overseer
    }

    public enum AIState
    {
        Patroling,
        Chasing,
        Attacking,
        Confused,
        Listening,
        SeekingHelp,
        Dead
    }

 
    public BotType botType;
    public AIState currentState = AIState.Patroling;
    public Brain brain;
    public Transform[] patrolPoints;
    public float detectionRange = 10f;
    public float attackRange = 5f;
    public float attackCooldown = 2f;
    public float helpThreshold = 20f;
    public float seekHelpRadius = 15f;
    public float health = 0f;
    public float maxHealth = 100f;
   
    public NavMeshAgent agent;
    public Animator animator;
    public Transform target;
    public LayerMask obstacleLayer;
    public LayerMask botsLayer;
    public PlayerHealth playerHealth;

    private int currentPatrolIndex = 0;
    private bool isDead = false;
    private bool isPaused = false;
    private Blaster blaster;
    private bool canAttack = true;
    private float lastConfusedTime;

    [SerializeField] private float blasterFireAnimDelay;
    [SerializeField] private float faceTargetSpeed = 5f;
    [SerializeField] private float confusionCooldown = 2f;
    public PowerLaser powerLaser;
    public Image healthFillImage;

    void Start()
    {
        health = maxHealth;
        UpdateHealthUI();
        if (brain == null)
        {
            Debug.LogError("Brain not assigned!");
            return;
        }

        blaster = GetComponentInChildren<Blaster>();
        agent.speed = botType switch
        {
            BotType.Drone => brain.walkSpeed * 1.2f,
            BotType.Enforcer => brain.walkSpeed,
            BotType.Overseer => brain.walkSpeed * 0.8f,
            _ => brain.walkSpeed
        };

        if (patrolPoints.Length > 0)
        {
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

  
    void Update()
    {
        if (isDead) return;

        switch (currentState)
        {
            case AIState.Patroling:
                Patrol();
                break;
            case AIState.Chasing:
                if(botType == BotType.Overseer)
                {
                    powerLaser.gameObject.SetActive(false);
                    powerLaser.transform.localScale = new Vector3(1f, 1f, 0f);
                }
                Chase();
                break;
            case AIState.Attacking:
                Attack();
                break;
            case AIState.Confused:
                Confused();
                break;
            case AIState.Listening:
                Listen();
                break;
            case AIState.SeekingHelp:
                SeekHelp();
                break;
            case AIState.Dead:
                Die();
                break;
        }

        UpdateAnimator();
        UpdateHealthUI();
    }
  
    
    void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 2f && !isPaused)
        {
            StartCoroutine(PauseAndResumePatrol());
        }

        CheckForTarget();
    }

    void Chase()
    {
        if (target == null || playerHealth == null || playerHealth.IsDead || !CanSeeTarget())
        {
            // Enter Confused state with a cooldown
            if (Time.time - lastConfusedTime > confusionCooldown)
            {
                currentState = AIState.Confused;
                lastConfusedTime = Time.time;
            }
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget <= attackRange)
        {
            currentState = AIState.Attacking;
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(target.position);
    }

    private IEnumerator PauseAndResumePatrol()
    {
        isPaused = true;
        animator.SetBool("IsWalking", false);
        yield return new WaitForSeconds(Random.Range(3f, 5f));
        animator.SetBool("IsWalking", true);
        isPaused = false;
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
    }

    void Attack()
    {
        if (target == null || !CanSeeTarget() || playerHealth == null || playerHealth.IsDead)
        {
            currentState = AIState.Patroling;
            agent.isStopped = false;
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget > attackRange)
        {
            currentState = AIState.Chasing;
            return;
        }

        agent.isStopped = true;

        FaceTarget();

        if (canAttack)
        {
            StartCoroutine(PerformAttack());
        }
    }

    private bool CanSeeTarget()
    {
        if (target == null) return false;

        // Ensure the correct obstacle layer is being checked
        if (Physics.Linecast(transform.position + Vector3.up, target.position + Vector3.up, out RaycastHit hit, obstacleLayer))
        {
            return hit.transform == target;
        }

        return true;
    }

    private void FaceTarget()
    {
        if (target == null) return;

        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * faceTargetSpeed);
    }

    private IEnumerator PerformAttack()
    {
        canAttack = false;

        if(botType== BotType.Overseer)
        {
            animator.SetTrigger("Attack2");
        }else if(botType== BotType.Enforcer)
        {
            animator.SetTrigger("Attack");
        }
        

        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
    }

    void Confused()
    {
       
        StartCoroutine(HandleConfusion());
    }

    private IEnumerator HandleConfusion()
    {
        animator.SetTrigger("Confused");
        agent.isStopped =true;
        yield return new WaitForSeconds(2f);
        currentState = AIState.Patroling;
        agent.isStopped = false;
        if (patrolPoints.Length > 0)
        {
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    void Listen()
    {
      //Not enough time to add more features here, sorry...
    }

    void SeekHelp()
    {
        Collider[] allies = Physics.OverlapSphere(transform.position, seekHelpRadius, botsLayer);
        foreach (var ally in allies)
        {
           //Here i was thinking to add feature that bot can run and ask some help...
        }

        currentState = AIState.Patroling;
    }
    private void UpdateHealthUI()
    {
        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = (float)health / maxHealth;
        }
    }
    void Die()
    {
        animator.SetTrigger("Die");
        agent.isStopped = true;
        agent.enabled = false;
        currentState = AIState.Dead;
        isDead = true;
     
        Destroy(gameObject, 5f);
    }

    void CheckForTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {

              
                Transform potentialTarget = collider.transform;

                //playerHealth = target.GetComponent<PlayerHealth>();

                //if (playerHealth.IsDead)
                //{
                //    target = null;
                //    return;
                //}

                if (IsTargetVisible(potentialTarget))
                {
                    target = potentialTarget;
                    playerHealth = target.GetComponent<PlayerHealth>();
                    if (playerHealth.IsDead)
                    {
                        target = null;
                    }
                    currentState = AIState.Chasing;
                    return;
                }
            }
        }

        
        target = null;
        playerHealth = null;
    }

    private bool IsTargetVisible(Transform potentialTarget)
    {
        if (Physics.Linecast(transform.position + Vector3.up, potentialTarget.position + Vector3.up, out RaycastHit hit, obstacleLayer))
        {
            return hit.transform == potentialTarget;
        }

        return true;
    }

    private void OnDrawGizmos()
    {
        if (target == null) return;

        Gizmos.color = CanSeeTarget() ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position + Vector3.up, target.position + Vector3.up);
    }

    void UpdateAnimator()
    {
        switch (currentState)
        {
            case AIState.Dead:
                animator.SetTrigger("Die");
                break;
            case AIState.Patroling:
                animator.SetBool("IsWalking", true);
                animator.SetBool("IsRunning", false);
                break;
            case AIState.Chasing:
                animator.SetBool("IsWalking", false);
                animator.SetBool("IsRunning", true);
                break;
            case AIState.Attacking:
                animator.SetBool("IsWalking", false);
                animator.SetBool("IsRunning", false);

                if(botType == BotType.Enforcer)
                {
                    animator.SetBool("PistolIdle", true);
                }else if(botType == BotType.Overseer)
                {
                    animator.SetBool("IsIdling", true);
                }
                break;
            case AIState.Confused:
                animator.SetTrigger("Confused");
                break;
           
            default:
                animator.SetBool("IsWalking", false);
                animator.SetBool("IsRunning", false);
                break;
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            currentState = AIState.Dead;
        }
        else if (health < helpThreshold)
        {
            currentState = AIState.SeekingHelp;
        }
    }
}
