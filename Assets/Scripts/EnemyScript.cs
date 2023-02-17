using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [SerializeField]
    public enum EnemyState { Patrol, Follow, Attack, ReturnToPatrol, Dead };
    public EnemyState currentEnemyState;
    public Animator anim;
    public Vector3 leftPosition;
    public Vector3 rightPosition;
    public Vector3 enemyStartPosition;
    public Transform enemyTransform;
    public Transform enemyTarget;
    public float patrolDistance = 3;

    public Transform enemyAttackTrigger;
    public Vector2 enemyAttackWidthHeight;
    public LayerMask playerMask;
    bool isAttacking = false;
    public int enemyHealth;
    public float enemyCooldown;
    public float DeathCD;
    public AudioSource audioSource;




    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        enemyStartPosition = new Vector3(transform.position.x, transform.position.y);
        leftPosition = new Vector3(enemyStartPosition.x + patrolDistance, enemyStartPosition.y);
        rightPosition = new Vector3(enemyStartPosition.x - patrolDistance, enemyStartPosition.y);
        anim = GetComponent<Animator>();
        enemyTransform = GetComponent<Transform>();
        // Start Coroutine
        StartCoroutine(Move(leftPosition));
        


    }

    void SetCurrentEnemyState(EnemyState state)
    {
        currentEnemyState = state;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        switch (currentEnemyState)
        {
            case EnemyState.Patrol:
                float distToFollow = Vector2.Distance(enemyTransform.position, enemyTarget.position);

                if (distToFollow < 5)
                {
                    //Start follow played
                    anim.SetBool("EnemyAttack", false);
                    anim.SetBool("EnemyRun", true);
                    StopCoroutine("Move");
                    SetCurrentEnemyState(EnemyState.Follow);

                }

                break;

            case EnemyState.Follow:
                float distToAttack = Vector2.Distance(enemyTransform.position, enemyTarget.position);
                enemyTransform.position = Vector2.MoveTowards(enemyTransform.position, enemyTarget.position, 0.1f);

                //check direction of enemy
                if (transform.position.x < enemyTarget.position.x)
                {
                    Vector3 eXScale = transform.localScale;
                    eXScale.x = 1;
                    enemyTransform.localScale = eXScale;
                }
                else
                {
                    Vector3 eXScale = transform.localScale;
                    eXScale.x = -1;
                    transform.localScale = eXScale;
                }

                if (distToAttack < 1.5)
                {
                    //Start follow played
                    anim.SetBool("EnemyAttack", true);
                    anim.SetBool("EnemyRun", false);
                    SetCurrentEnemyState(EnemyState.Attack);

                }

                if (distToAttack > 7)
                {
                    //Start follow played
                    anim.SetBool("EnemyAttack", false);
                    anim.SetBool("EnemyRun", true);
                    SetCurrentEnemyState(EnemyState.ReturnToPatrol);

                }

                break;

            case EnemyState.Attack:
                float distToFollowAgain = Vector2.Distance(enemyTransform.position, enemyTarget.position);

                if (distToFollowAgain > 4)
                {
                    //Start follow player
                    anim.SetBool("EnemyAttack", false);
                    anim.SetBool("EnemyRun", true);
                    StopCoroutine("Move");
                    SetCurrentEnemyState(EnemyState.Follow);

                }
                else
                {
                    if (!isAttacking)
                    {
                        isAttacking = true;
                        StartCoroutine(EnemyAttack());
                    }
                }

                break;

            case EnemyState.ReturnToPatrol:
                enemyTransform.position = Vector2.MoveTowards(enemyTransform.position, enemyStartPosition, 0.01f);
                float distToFollowOnReturn = Vector2.Distance(enemyTransform.position, enemyTarget.position);

                if (distToFollowOnReturn > 4)
                {
                    //Start follow player
                    anim.SetBool("EnemyAttack", false);
                    anim.SetBool("EnemyRun", true);
                    StopCoroutine("Move");
                    SetCurrentEnemyState(EnemyState.Follow);

                }

                //check direction of enemy
                if (transform.position.x < enemyStartPosition.x)
                {
                    Vector3 eXScale = transform.localScale;
                    eXScale.x = 1;
                    transform.localScale = eXScale;
                }
                else
                {
                    Vector3 eXScale = transform.localScale;
                    eXScale.x = -1;
                    transform.localScale = eXScale;
                }

                //check tosee if her arrived start pos

                if (enemyTransform.position == enemyStartPosition)
                {
                    anim.SetBool("EnemyAttack", false);
                    anim.SetBool("EnemyRun", true);
                    StopCoroutine("Move");
                    Vector3 eXScale = transform.localScale;
                    eXScale.x = 1;
                    transform.localScale = eXScale;
                    StartCoroutine(Move(leftPosition));
                    SetCurrentEnemyState(EnemyState.Patrol);
                }

                break;

        }

        if (enemyHealth <= 0)
        {
            anim.SetBool("EnemyDead", true);
            anim.SetBool("EnemyAttack", false);
            anim.SetBool("EnemyRun", false);
            Destroy(gameObject, 0.5f);
        }

    }

    IEnumerator Move(Vector3 target)
    {
        while (Mathf.Abs((target.x - enemyTransform.localPosition.x)) > 0.2f)
        {
            //ternary operator
            Vector3 direction = target.x == rightPosition.x ? Vector2.left : Vector2.right;
            transform.localPosition += direction * Time.deltaTime;
            if (currentEnemyState == EnemyState.Attack)
            {
                StopCoroutine("Move");
                break;
            }
            yield return null;
        }

        //Pause enemy at waypoint
        yield return new WaitForSeconds(0.5f);
        Vector3 eXScale = enemyTransform.localScale;
        eXScale.x *= -1;

        enemyTransform.localScale = eXScale;
        //Setup next target
        Vector3 nextTarget = target.x == rightPosition.x ? leftPosition : rightPosition;
        StartCoroutine("Move", nextTarget);
    }

    IEnumerator EnemyAttack()
    {
        Collider2D[] enemyAttack = Physics2D.OverlapBoxAll(enemyAttackTrigger.position, enemyAttackWidthHeight, 0, playerMask);
        audioSource.Play();
        foreach (Collider2D player in enemyAttack)
        {
            
            var playerHealth = player.GetComponent<PlayerScript>().playerHealth;
            player.GetComponent<PlayerScript>().playerHearts[playerHealth - 1].SetActive(false);
            player.GetComponent<PlayerScript>().playerHealth -= 1;
        }

        yield return new WaitForSeconds(enemyCooldown);

        isAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(enemyAttackTrigger.position, enemyAttackWidthHeight);
    }
}
