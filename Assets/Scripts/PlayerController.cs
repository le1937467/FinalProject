using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float movePower = 10f;
    private int direction = 1;
    Vector3 movement;

    [Header("Jump")]
    public float jumpPower = 15f; //Set Gravity Scale in Rigidbody2D Component to 5
    public bool isJumping = false;

    [Header("Rigidbody")]
    public Rigidbody2D rb;
    private Animator anim;

    [Header("States")]
    public bool canJump = true;
    private bool alive = true;
    private bool canAttack = true;
    public bool canMove = true;
    public bool canHit = false;

    [Header("Animations")]
    public Animator transition;
    public float transitionTime = 1f;

    [Header("Particles")]
    public ParticleSystem dust;

    [Header("Render")]
    [SerializeField]
    private LayerMask groundLayer;

    [SerializeField]
    private GameObject fireball;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

    }

    private void Update()
    {
        Restart();
        if (alive)
        {
            Attack();
            RangedAttack();
            Jump();
            Run();

            if (!Input.anyKey) // Player should not move X & Z if no input
            {
                FreezePosition();
            }
        }
    }

    void Run()
    {
        if (!canMove)
            return;


        Vector3 moveVelocity = Vector3.zero;
        anim.SetBool("isRun", false);


        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            direction = -1;
            moveVelocity = Vector3.left;

            transform.localScale = new Vector3(direction, 1, 1);
            if (!anim.GetBool("isJump"))
                anim.SetBool("isRun", true);

        }
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            direction = 1;
            moveVelocity = Vector3.right;

            transform.localScale = new Vector3(direction, 1, 1);
            if (!anim.GetBool("isJump"))
                anim.SetBool("isRun", true);

        }
        transform.position += moveVelocity * movePower * Time.deltaTime;
    }
    bool isGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - 0.2f), -Vector2.up, groundLayer);
        if (hit.collider != null)
        {
            float distance = Mathf.Abs(hit.point.y - transform.position.y);
            if (distance < 0.3f)
                return true;
            else
                return false;
        }
        else
            return false;
    }
    void Jump()
    {
        if (isGrounded())
        {
            if (!canMove)
                canMove = true;
            canJump = true;
            isJumping = false;
            anim.SetBool("isJump", false);
        }
        if ((Input.GetKey(KeyCode.Space) || Input.GetAxisRaw("Vertical") > 0)
        && !isJumping && canJump)
        {
            canJump = false;
            isJumping = true;
            anim.SetBool("isJump", true);
            rb.velocity = Vector2.zero;
            Vector2 jumpVelocity = new Vector2(0, jumpPower);
            rb.AddForce(jumpVelocity, ForceMode2D.Impulse);
        }

    }
    void Attack()
    {
        if (Input.GetKeyDown(KeyCode.E) && canAttack)
        {
            anim.SetTrigger("attack");
            StartCoroutine(WaitForAttackState());
        }
    }
    void RangedAttack()
    {
        if (Input.GetKeyDown(KeyCode.R) && canAttack)
        {
            anim.SetTrigger("attack");
            if (direction == 1)
            {
                GameObject go = Instantiate(fireball, new Vector3(transform.position.x, transform.position.y + 3f), Quaternion.identity);
                go.GetComponent<Rigidbody2D>().velocity = Vector2.right * 20f;
                go.transform.localScale = new Vector2(-go.transform.localScale.x, go.transform.localScale.y);

            }
            else
            {
                GameObject go = Instantiate(fireball, new Vector3(transform.position.x, transform.position.y + 3f), Quaternion.identity);
                go.GetComponent<Rigidbody2D>().velocity = Vector2.left * 20f;
            }
            StartCoroutine(WaitForAttackState());
        }
    }
    public void Hurt(float str)
    {
        anim.SetTrigger("hurt");
        if (direction == 1)
            rb.AddForce(new Vector2(-5f * str, 2f), ForceMode2D.Impulse);
        else
            rb.AddForce(new Vector2(5f * str, 2f), ForceMode2D.Impulse);
    }
    public void Die()
    {
        anim.SetTrigger("die");
        alive = false;
        gameObject.tag = "dead";
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex));

    }
    void FreezePosition()
    {
        rb.constraints = RigidbodyConstraints2D.FreezePositionX;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    void Restart()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            anim.SetTrigger("idle");
            alive = true;
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "NextScene")
        {
            StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
        }

        if (collision.gameObject.tag == "Out-Of-Bound")
        {
            StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex));
        }
    }
    IEnumerator LoadLevel(int levelIndex)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelIndex);
    }
    IEnumerator WaitForAttackState()
    {
        canAttack = false;
        canHit = true;
        yield return new WaitForSeconds(0.2f);
        yield return new WaitUntil(() => !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);
        canAttack = true;
        canHit = false;
    }
}
