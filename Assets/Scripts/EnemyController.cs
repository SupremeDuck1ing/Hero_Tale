using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DevionGames.StatSystem; 
using DevionGames.QuestSystem;
using UnityEngine.Events;
using DevionGames.InventorySystem;


public class WanderSpot {
    public GameObject Center;
    public float WanderRadius;
}
public class EnemyController : MonoBehaviour
{
    [field: Header("Wander Settings")]
    public bool enableWandering = false;
    public float minWanderRadius = 0f;
    public float maxWanderRadius = 10f;
    public float wanderTimer = 5f;
    public List<Transform> Spots;

    [field: Header("Debug Settings")]
    public bool showRayCast = false;
    public bool showNavMeshPath = false;
    public bool eneableDebugMessages;

    [field: Header("Enemy AI Settings")]
    public bool willRespawn = false;
    public float respawnTime = 10f;
    public float attackDelay = 1.0f;
    public int experienceReward = 30;
    public bool returnToOrigin = false;
    public float attackDistance = 2f;
    protected float walkSpeed = 1f;
    protected float runSpeed = 1f;
    public TestBoxRadius Radius;
    public UnityEvent onDeath;
    public UnityEvent onTrigger;
    protected bool testBoxTriggered;
    protected NavMeshAgent agent;
    protected GameObject player;
    protected Animator animator;
    protected bool isMoving = false;
    protected bool isAttacking = false;
    protected bool isTargetingPlayer = false;
    protected bool isReturning = false;
    protected bool isCasting = false;
    protected bool isCastOnCoolDown;
    [field: Header("Audio Settings")]
    public AudioClip[] footsteps;
    public AudioClip[] grunts;
    protected AudioSource audioSource;

    [HideInInspector]
    public bool isDead;

    [SerializeField]
    protected float distanceFromRadius;
    [SerializeField]
    protected float distanceFromPlayer;

    protected Stats PlayerStats; 
    
    protected Transform PlayerLocation;
    protected Transform GuardLocation;
    

    protected Vector3 origin;

    protected Vector3 originalOrientation;
    protected Vector3 originalPosition;
    protected StatsHandler handler;
    protected float logTime = 1f;
    LineRenderer lineRenderer;
    protected bool hitAnimating;
    protected bool isShowingHealthBar;
    protected bool isOnStunCooldown;
    protected float currentHealth;
    protected float baseHealth;
    private Transform wanderSphereTarget;
    private float timer;
    private bool isAggroed;
    private bool generatingPosition;
    private bool isTimerReset = true;


    protected virtual void Awake()
    {
        timer = wanderTimer;

        if (showNavMeshPath)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        handler = GetComponent<StatsHandler>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        originalOrientation = transform.eulerAngles;
        originalPosition = transform.position;
        Debug.Assert(Radius != null);
        GuardLocation = Radius.gameObject.GetComponent<Transform>();

        
        player = GameObject.FindGameObjectWithTag("Player");
        PlayerStats = FindObjectOfType<Stats>(); 
        PlayerLocation = player.GetComponent<Transform>();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = 1f;
        audioSource.spatialBlend = 1f;
        audioSource.maxDistance = 30f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
    }

    // Start is called before the first frame update
    protected void Start()
    {
        StartCoroutine(LifeCheck());
        origin = transform.position;
        baseHealth = currentHealth = handler.GetStatValue("Health");
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!isDead)
        {
            distanceFromRadius = Vector3.Distance(PlayerLocation.position, GuardLocation.position);
            distanceFromPlayer = Vector3.Distance(PlayerLocation.position, transform.position);

            if (showNavMeshPath)
            {
                ShowNavMeshPathLine();
            }

            // Wander state.
            if (enableWandering && !isAggroed)
            {
                timer += Time.deltaTime;
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    animator.SetFloat("speedh", 0f);
                }
                if (timer >= wanderTimer && !generatingPosition && isTimerReset) {
                    if (animator.GetFloat("speedh") == 0f)
                    {
                        animator.SetFloat("speedh", 1f);
                    }
                    StartCoroutine(GeneratePosition());
                }
            }


            // initial State when player enters trigger radius
            if (!isPlayerInMeleeRange() && isPlayerInRadius() && !isTargetingPlayer && !isAttacking && !testBoxTriggered && !isCasting && !hitAnimating)
            {
                isAggroed = true;
                testBoxTriggered = true;
                onTrigger?.Invoke();
                Grunt();
                StopCoroutine(TimerController());
                StartCoroutine(UpdateTargetCoroutine());
            }
            
            // follow state
            if (!isPlayerInMeleeRange() && !isTargetingPlayer && !isAttacking && testBoxTriggered && !isCasting && !hitAnimating)
            {
                StartCoroutine(UpdateTargetCoroutine());
            }
            
            // return state (unused)
            if (!isPlayerInRadius() && !isInOriginalPosition() && !isReturning && !isAttacking && returnToOrigin && !isCasting && !hitAnimating)
            {
                if (isTargetingPlayer)
                {
                    StopCoroutine(UpdateTargetCoroutine());
                }
                StartCoroutine(ReturnToOrigin());
            }

            // attack state
            if (isPlayerInMeleeRange() && !isAttacking && !isReturning && !isTargetingPlayer && !isCasting && !hitAnimating)
            {
                StartCoroutine(Attack());
            }

            // received hit (stops attack and follow state)
            if (isHit() && !hitAnimating && !isOnStunCooldown)
            {
                // if (!isShowingHealthBar)
                // {
                //     //ShowHealthAndName();
                // }
                StopCoroutine(UpdateTargetCoroutine());
                StopCoroutine(Attack());
                HitAnimation();
            }
        }
    }

    public Vector3 RandomNavSphere(float dist)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        var newSpot = Spots[Random.Range(0, Spots.Count)];
        randDirection += newSpot.transform.position;
        NavMeshHit navHit;
        NavMesh.SamplePosition (randDirection, out navHit, dist, -1);
        return navHit.position;
    }

    IEnumerator GeneratePosition()
    {
        generatingPosition = true;
        if (eneableDebugMessages)
            Debug.Log("GeneratePosition()");
        Vector3 newPos = RandomNavSphere(maxWanderRadius);
        while (Vector3.Distance(newPos, transform.position) <= minWanderRadius)
        {
            newPos = RandomNavSphere(maxWanderRadius);
            yield return null;
        }
        animator.SetFloat("speedh", walkSpeed);
        agent.SetDestination(newPos);
        timer = 0;
        StartCoroutine(TimerController());
        if (eneableDebugMessages)
            Debug.Log("Exiting GeneratePosition()");
        generatingPosition = false;
        yield return null;
    }

    IEnumerator TimerController()
    {
        isTimerReset = false;
        if (eneableDebugMessages)
            Debug.Log("TimerController()");
        Debug.Log(agent.isStopped);
        Debug.Log(agent.destination);
        while (Vector3.Distance(transform.position, agent.destination) > 1.6f)
        {
            timer = 0;
            yield return null;
        }
        if (eneableDebugMessages)
            Debug.Log("Exiting TimerController()");
        isTimerReset = true;
    }

    protected void ShowHealthAndName()
    {
        GameObject canvasObject = GetComponentInChildren<Canvas>().gameObject;
        canvasObject.transform.GetChild(0).gameObject.SetActive(true);
        isShowingHealthBar = true;
    }

    protected void HitAnimation()
    {
        if (!testBoxTriggered)
        {
            testBoxTriggered = true;
        }
        StartCoroutine(HitAnimate());
    }

    IEnumerator HitAnimate()
    {
        hitAnimating = true;
        if (eneableDebugMessages)
            Debug.Log("HitAnimate()");
        StartCoroutine(StunCooldown());
        //animator.SetTrigger("Hit1");
        animator.SetTrigger("Hit1");
        yield return new WaitForEndOfFrame();
        yield return new WaitWhile(isInHitAnimation);
        if (eneableDebugMessages)
            Debug.Log("Exiting HitAnimate()");
        hitAnimating = false;
    }

    IEnumerator StunCooldown()
    {
        isOnStunCooldown = true;
        yield return new WaitForSeconds(4f);
        isOnStunCooldown = false;
    }

    protected IEnumerator UpdateTargetCoroutine()
    {
        isTargetingPlayer = true;
        agent.isStopped = false;
        if (eneableDebugMessages)
            Debug.Log("UpdateTargetCoroutine()");
        yield return new WaitWhile(isInAttackAnimation);
        animator.SetFloat("speedh", runSpeed);
        
        while (!isPlayerInMeleeRange() && !isDead)
        {
            agent.destination = player.transform.position;
            yield return null;
        }
        agent.isStopped = true;
        if (eneableDebugMessages)
            Debug.Log("Exited UpdateTargetCoroutine()");
        isTargetingPlayer = false;
    }

    IEnumerator ReturnToOrigin()
    {
        isReturning = true;
        if (eneableDebugMessages)
            Debug.Log("ReturnToOrigin()");
        animator.SetFloat("speedh", walkSpeed);
        agent.destination = origin;
        yield return new WaitUntil(isInOriginalPosition);
        yield return new WaitForSeconds(0.2f);
        animator.SetFloat("speedh", 0.0f);
        yield return new WaitForSeconds(1);
        transform.eulerAngles = originalOrientation;
        testBoxTriggered = false;
        if (eneableDebugMessages)
            Debug.Log("Exited ReturnToOrigin()");
        isReturning = false;
    }

    protected IEnumerator Attack()
    {
        isAttacking = true;
        if (eneableDebugMessages)
            Debug.Log("Attack()");
        animator.SetFloat("speedh", 0.0f);
        agent.isStopped = true;
        StartCoroutine(FacePlayer());
        while (isPlayerInMeleeRange() && !isDead)
        {
            animator.SetTrigger("Attack1h1");
            yield return new WaitForSeconds(attackDelay);
        }
        agent.isStopped = false;
        if (eneableDebugMessages)
            Debug.Log("Exited Attack()");
        isAttacking = false;
    }

    IEnumerator FacePlayer()
    {
        if (eneableDebugMessages)
            Debug.Log("FacePlayer()");
        while (isAttacking || isTargetingPlayer && !isDead)
        {
            transform.LookAt(player.transform.position);
            yield return null;
        }
        if (eneableDebugMessages)
            Debug.Log("Exited FacePlayer()");
    }

    protected virtual IEnumerator LifeCheck()
    {
        if (eneableDebugMessages)
            Debug.Log("LifeCheck()");
        while (handler.GetStatCurrentValue("Health") > 0)
        {
            yield return null;
        }
        isDead = true;
        StopCoroutine(UpdateTargetCoroutine());
        agent.isStopped = true;
        //agent.destination = transform.position;
        StopCoroutine(Attack());
        StopCoroutine(FacePlayer());
        
        animator.SetTrigger("Fall1");
        player.GetComponent<Stats>().GainXp(experienceReward);
        onDeath?.Invoke();
        GetComponent<Collider>().enabled = false;

        yield return new WaitForSeconds(3f);

        for(int i = 0; i < transform.childCount; ++i)
        {
            transform.GetChild(i).gameObject.SetActive(false); // or false
        }

        if (willRespawn)
        {
            isTargetingPlayer = false;
            isAttacking = false;
            isCasting = false;
            isAggroed = false;
            StartCoroutine(Respawn());
        }
        else
        {
            this.enabled = false;
        }
        if (eneableDebugMessages)
            Debug.Log("Exiting LifeCheck()");
        //StopAllCoroutines();
    }

    IEnumerator Respawn()
    {
        if (eneableDebugMessages)
            Debug.Log("Respawn()");
        agent.isStopped = true;
        yield return new WaitForSeconds(respawnTime);
        testBoxTriggered = false;
        isDead = false;
        transform.position = originalPosition;
        GetComponent<Collider>().enabled = true;

        animator.SetTrigger("Up");

        for(int i = 0; i < transform.childCount; ++i)
        {
            if (transform.GetChild(i).gameObject.name == "Selection")
                continue;
            transform.GetChild(i).gameObject.SetActive(true); // or false
        }

        agent.destination = originalPosition;

        GetComponent<StatsHandler>().ApplyDamage("Health", -baseHealth);
        currentHealth = baseHealth;
        StopCoroutine(UpdateTargetCoroutine());
        agent.isStopped = false;
        StartCoroutine(LifeCheck());
        isAggroed = false;
        if (eneableDebugMessages)
            Debug.Log("Exiting Respawn()");
    }

    protected void ShowNavMeshPathLine()
    {
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, agent.destination, NavMesh.AllAreas, path);
        Vector3[] corners = path.corners;
        lineRenderer.SetPositions(corners);
    }

    public void DropItem(GameObject item)
    {
        GameObject droppedItem = Instantiate(item, null);
        //droppedItem.transform.localScale = new Vector3(1,1,1);
        droppedItem.transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        //droppedItem.GetComponent<Rigidbody>().isKinematic = true;
        //droppedItem.gameObject.GetComponentInChildren<SphereCollider>().center = new Vector3(0,0,0);
        StartCoroutine(FixDroppedItem(droppedItem));
    }

    IEnumerator FixDroppedItem(GameObject droppedItem)
    {
        yield return new WaitForSeconds(0.2f);
        droppedItem.GetComponentInChildren<SphereCollider>().center = new Vector3(0,0,0);
    }

    IEnumerator LerpAnimation(float startValue, float endValue, float lerpDuration)
    {
        float valueToLerp;
        float timeElapsed = 0;

        while (timeElapsed < lerpDuration)
        {
            valueToLerp = Mathf.Lerp(startValue, endValue, timeElapsed / lerpDuration);
            animator.SetFloat("speedh", valueToLerp);
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        valueToLerp = endValue;
    }


    protected bool isPlayerInMeleeRange()
    {
        return Vector3.Distance(player.transform.position, transform.position) <= attackDistance;
    }

    bool isInOriginalPosition()
    {
        // navMesh may not return to approximate origin, so truncate the float.
        int distFromOrigin = (int)Vector3.Distance(origin, transform.position);
        return distFromOrigin <= 1;
    }

    bool isPlayerInRadius()
    {
        return distanceFromRadius <= Radius.radius;
    }

    bool isInAttackAnimation()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1h1");
    }

    bool isInHitAnimation()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Hit1");
    }

    bool isHit()
    {
        if (handler.GetStatCurrentValue("Health") < currentHealth)
        {
            currentHealth = handler.GetStatCurrentValue("Health");
            return true;
        }
        return false;
        //return handler.GetStatCurrentValue("Health") < handler.GetStatValue("Health");
    }

    void playSwingSound() 
    {
        FindObjectOfType<AudioManager>().Play("SkeletonAttacks");
    }

    protected void Step()
    {
        AudioClip clip = GetRandomFootstepClip();
        audioSource.PlayOneShot(clip);
    }

    protected virtual void Grunt()
    {
        AudioClip clip = GetRandomGruntClip();
        audioSource.PlayOneShot(clip);
    }
    protected AudioClip GetRandomGruntClip()
    {
        return grunts[UnityEngine.Random.Range(0,grunts.Length)];
    }

    protected AudioClip GetRandomFootstepClip()
    {
        return footsteps[UnityEngine.Random.Range(0,footsteps.Length)];
    }
}
