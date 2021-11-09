using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUniversalController : MonoBehaviour
{

    [SerializeField] private float speed = 5f;
    [SerializeField] private float damage = 30f;
    [SerializeField] public float maxHealth = 1f;
    [SerializeField] private float knockbackStrength = 1f;


    private EnemyHealthBarScript healthBar;
    private float currentHealth;
    private Rigidbody2D rb;
    private Animator anim;

    private bool facingLeft = true;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        healthBar = GetComponentInChildren<EnemyHealthBarScript>(true);
        currentHealth = maxHealth;

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            //Player is in range
            if(collision.transform.position.x < transform.position.x)
            {
                //Player is to our left
                rb.velocity = new Vector2(-speed,0);
                    anim.SetBool("isRunning", true);
                if (!facingLeft)
                {
                    transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
                    facingLeft = true;
                }
            }
            else
            {
                //Player is to our right
                rb.velocity = new Vector2(speed, 0);
                    anim.SetBool("isRunning", true);
                if (facingLeft)
                {
                    transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
                    facingLeft = false;
                }
            }

        }
        else if(collision.tag == "dead")
        {
            rb.velocity = Vector2.zero;
            anim.SetBool("isRunning", false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player" || collision.tag == "dead")
        {
            rb.velocity = new Vector2(0,0);
            anim.SetBool("isRunning", false);
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            anim.SetTrigger("attack");
            PlayerCombat.GetInstance().DamagePlayer(damage, knockbackStrength);
        }
    }

    public void TakeDamage(float damage)
    {
        this.currentHealth -= damage;
        if(currentHealth <= 0)
        {
            //Die
            Destroy(gameObject);
        }
        else
        {
            healthBar.TakeDamage(damage);
        }
    }

}
