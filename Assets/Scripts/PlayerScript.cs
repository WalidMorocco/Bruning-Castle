using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    float h;
    float v;
    public int speed;
    Vector2 dir;
    bool facingRight = true;
    public int itemsCollected;
    Animator anim;

    public Transform attackTrigger;
    public Vector2 attackWidthHeight;
    public LayerMask enemyLayer;
    bool isAttacking = false;
    public int playerHealth;
    public GameObject[] playerHearts;

    public AudioSource audioSource;



    [SerializeField]
    int itemsInScene;

    [SerializeField]
    GameObject door;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(dir * Time.fixedDeltaTime * speed);

        if (itemsCollected >= itemsInScene)
        {
            Destroy(door);
        }

        if (playerHealth <= 0)
        {
            anim.SetBool("isDead", true);
            anim.SetBool("isAttacking", false);
            anim.SetBool("isRunning", false);
        }


    }

    public void MovePlayer(InputAction.CallbackContext context)
    {
        dir = context.ReadValue<Vector2>();
        h = dir.x;
        v = dir.y;
        FlipPlayer(h);
        if (h !=0 || v != 0)
        {
            anim.SetBool("isRunning", true);
        }
        else
        {
            anim.SetBool("isRunning", false);
        }
        
    }

    private void FlipPlayer(float h)
    {
        if (h>0 && !facingRight || h<0 && facingRight)
        {
            facingRight = !facingRight;
            Vector3 playerXScale = transform.localScale;
            playerXScale.x *= -1;
            transform.localScale = playerXScale;
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            anim.SetTrigger("isAttacking");
        }
    }

    public void OnAttack()
    {
        Collider2D[] playerAttack = Physics2D.OverlapBoxAll(attackTrigger.position, attackWidthHeight, 0, enemyLayer);
        audioSource.Play();
        foreach (Collider2D enemy in playerAttack)
        {
            enemy.GetComponent<EnemyScript>().enemyHealth -= 1;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(attackTrigger.position, attackWidthHeight);
    }

    public void LoadNewScene()
    {
        SceneManager.LoadScene("GameOver");
    }
}
