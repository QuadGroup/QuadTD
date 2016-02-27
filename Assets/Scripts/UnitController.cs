using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum UnitType
{
    Enemy,
    Guard
}

public class UnitController : MonoBehaviour {

    public float health;
    public float agrRadius;
    public float attackRadius;
    public float attackDelay;
    public float attack;
    public UnitType unitType;
    public UnitController target;
    public List<UnitController> targets;
    public AstarAI astarAI;
    public Vector3 endPoint;
    Animator animator;
    public bool isAgr = false;
    public bool isAttack = false;
    public bool isMove = false;
    public bool isIdle = false;
    public bool isDead = false;
    public float currentHealth;

    FXController fxController;
    AudioController audioController;
    public WaveManager waveManager;

	// Use this for initialization
	void Start () {
        fxController = GetComponent<FXController>();
        audioController = GetComponent<AudioController>();
        currentHealth = health;
        GetComponent<SphereCollider>().radius = agrRadius;
        animator = GetComponent <Animator>();
        if (unitType == UnitType.Enemy)
        MoveToEnd();
	}

    void Update()
    {
        if (target != null && !isDead)
        {
            if (Vector3.Distance(transform.position, target.transform.position) <= attackRadius && !isAttack)
            {
                Attack();
            }
            if (Vector3.Distance(transform.position, target.transform.position) > attackRadius && isAttack)
            {
                Debug.Log("Idle");
                Idle();
            }
            if (unitType == UnitType.Enemy && Vector3.Distance(transform.position, endPoint) < 10)
            {
                Debug.Log("End Point death");
                Death();
            }
        }
        
    }
    void OnTriggerEnter(Collider other)
    {

        if ((unitType == UnitType.Enemy && other.gameObject.layer == LayerMask.NameToLayer("Guard")) || (unitType == UnitType.Guard && other.gameObject.layer == LayerMask.NameToLayer("Enemy")))
        {

            targets.Add(other.GetComponent<UnitController>());
            target = other.GetComponent<UnitController>();

            astarAI.targetPosition = target.transform.position;
            astarAI.ChangePath();

            
            if (isAgr == false)
            {
                isAgr = true;
                StartCoroutine(AttackGuardsStack());
            }
        }
    }

    IEnumerator AttackGuardsStack()
    {
        while (targets.Count > 0)
        {
            if (target.currentHealth <= 0)
            {
                targets.Remove(target);
                if (target != null)
                {
                    Debug.Log("set death");
                    target.Death();
                    target = null;
                }
                if (targets.Count > 0)
                {
                    target = targets[targets.Count - 1];
                    if (target != null)
                    {
                        astarAI.targetPosition = target.transform.position;
                        astarAI.ChangePath();
                    }
                }
            }
            if (target != null)
            {
                yield return new WaitForSeconds(1f);
            }

        }
        Debug.Log("Finish:"+targets.Count);
        isAgr = false;
        if (unitType == UnitType.Enemy)
        MoveToEnd();
        yield return null;
    }

    IEnumerator Attacking()
    {
        Debug.Log("Atacking");
        while (isAttack)
        {
            if (target.currentHealth > 0)
            {
                animator.SetTrigger("Attack");
                yield return new WaitForEndOfFrame();
                yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(1).length);
                if(target!=null)
                target.hit(attack);
                
                yield return new WaitForSeconds(attackDelay);
            }
            else break;
        }
        yield return null;
    }

    public void hit(float damage)
    {
        if (!isDead)
        {
            audioController.Hit();
            currentHealth -= damage;
            Debug.Log("unit:" + name + "currentHealth:" + currentHealth);
            fxController.Hit();
        }
    }

    public void Death()
    {
        animator.SetTrigger("Death");
        audioController.Death();
        isDead = true;
        //Destroy(gameObject);
        isIdle = false;
        isMove = false;
        isAttack = false;

        if(unitType == UnitType.Enemy)
            waveManager.enemiesCounter--;
        if (unitType == UnitType.Guard)
        {
            GetComponentInParent<SpawnPointGuard>().isEmpty = true;
        }
    }
    public void Idle()
    {
        if (!isDead)
        {
            animator.SetTrigger("Idle");
            isIdle = true;
            isMove = false;
            isAttack = false;
        }
    }
    public void Move()
    {
        if (!isDead)
        {
            animator.SetTrigger("Move");
            isIdle = false;
            isMove = true;
            isAttack = false;
        }
       
    }
    public void Attack()
    {
        if (!isDead)
        {
            isIdle = false;
            isMove = false;
            isAttack = true;
            animator.SetTrigger("Idle");
            StartCoroutine(Attacking());
        }
    }
    public void MoveToEnd()
    {
        if (!isDead)
        {
            astarAI.targetPosition = endPoint;
            astarAI.ChangePath();
            Move();
        }
    }

}
