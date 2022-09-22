using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevionGames.StatSystem;
using UnityEngine.AI;
using UnityEngine.Events;
using DevionGames.InventorySystem;

public class OrcMageController : OrcController
{
    [field: Header("Debug Ranged Settings")]
    // private new float walkSpeed = 0.45f;
    // private new float runSpeed = 0.77f;
    protected bool isPerformingTriggerSequence = false;
    protected bool isSlowedDown;
    float Reach = 20f;
    bool canAttack = true;
    Vector3 fwd;
    RaycastHit hit;
    [field: Header("Projectile Settings")]
    public Transform projectileOrigin;
    public float fCastCoolDown = 5f;
    public GameObject castObject;
    public GameObject castOrigin;

    //Trigger devGamesTrig;
    protected override void Awake()
    {
        base.Awake();
        //devGamesTrig = GetComponent<Trigger>();
        Debug.Assert(projectileOrigin != null);
    }

    protected override void Update()
    {
        if (!isPlayerInMeleeRange() && !isCastOnCoolDown && !isCasting && !isAttacking && testBoxTriggered)
        {            
            if (IsPlayerInLOS())
            {
                StartCoroutine(CastSpell());
            }
            // else
            // {
            //     Debug.Log("Player not in LOS");
            // }            
        }

        if (!isPerformingTriggerSequence)
        {
            base.Update();
        }

        if (showRayCast)
        {
            fwd = transform.forward;
            Debug.DrawRay(projectileOrigin.position, fwd * Reach, Color.red);
        }
        

        // if (Input.GetKeyDown(KeyCode.J))
        // {
        //     if (!isSlowedDown)
        //     {
        //         isSlowedDown = true;
        //         Time.timeScale = 0.1f;
        //     }
        //     else
        //     {
        //         isSlowedDown = false;
        //         Time.timeScale = 1f;
        //     }
        // }


        //if (Input.GetKeyDown(KeyCode.H))
        
    }

    IEnumerator CastSpell()
    {
        isCasting = true;
        //Debug.Log("Player in sight");
        if (eneableDebugMessages)
            Debug.Log("CastSpell()");
        isPerformingTriggerSequence = true;
        isAttacking = false;
        isTargetingPlayer = false;
        agent.isStopped = true;
        agent.destination = transform.position;
        StartCoroutine(CastCoolDown());
        animator.SetFloat("speedh", 0);
        yield return new WaitForSeconds(0.5f);
        animator.SetTrigger("Cast");
        yield return new WaitForSeconds(0.2f);
        //devGamesTrig.Use();
        CastBehavior();
        yield return new WaitForSeconds(1f);
        if (eneableDebugMessages)
            Debug.Log("Exiting CastSpell()");
        isPerformingTriggerSequence = false;
        isCasting = false;
    }

    protected void CastBehavior()
    {
        Invoke("LookAtPlayer", 0);
        Instantiate(castObject, castOrigin.transform.position, castOrigin.transform.rotation);
    }

    IEnumerator CastCoolDown()
    {
        isCastOnCoolDown = true;
        if (eneableDebugMessages)
            Debug.Log("CastCoolDown()");
        var fTemp = fCastCoolDown;
        while (fCastCoolDown >= 0)
        {
            fCastCoolDown -= Time.deltaTime;
            yield return null;
        }
        fCastCoolDown = fTemp;
        if (eneableDebugMessages)
            Debug.Log("Exiting CastCoolDown()");
        isCastOnCoolDown = false;
    }

    // IEnumerator AttackCoolDown()
    // {
    //     isMeleeOnCooldown = false;
    //     while (fMeleeCoolDown >= 0f)
    //     {
    //         fMeleeCoolDown -= Time.deltaTime;
    //     }
    //     yield return new WaitForSeconds(3f);
    //     isMeleeOnCooldown = true;
    // }

    bool IsPlayerInLOS()
    {
        fwd = transform.forward;
        return Physics.Raycast(projectileOrigin.position, fwd, out hit, Reach) && hit.transform.tag == "Player";
    }

    public void IsTriggerInSequence(bool flag)
    {
        //isPerformingTriggerSequence = flag;
    }

    public void LookAtPlayer()
    {
        transform.LookAt(player.transform);
    }
}
