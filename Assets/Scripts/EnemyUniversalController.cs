using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUniversalController : MonoBehaviour
{

    [SerializeField] private float speed = 5f;
    [SerializeField] private float damage = 30f;
    [SerializeField] private float maxHealth = 1f;


    private float currentHealth;
    private Rigidbody2D rb;
    private Animator anim;

    private bool facingLeft = true;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
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
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            rb.velocity = new Vector2(0,0);
            anim.SetBool("isRunning", false);
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
    }

}
