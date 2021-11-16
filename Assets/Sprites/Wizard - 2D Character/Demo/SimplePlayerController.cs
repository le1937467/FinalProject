using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace ClearSky
{
    public class SimplePlayerController : MonoBehaviour
    {
        public float movePower = 10f;
        public float jumpPower = 15f; //Set Gravity Scale in Rigidbody2D Component to 5

        public Rigidbody2D rb;
        private Animator anim;
        Vector3 movement;
        private int direction = 1;
        public bool isJumping = false;
        public bool canJump = true;
        private bool alive = true;
        private bool canAttack = true;
        public bool canMove = true;
        public bool canHit = false;
        public Animator transition;
        public float transitionTime = 1f;

        [SerializeField]
        private LayerMask groundLayer;


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
                Jump();
                Run();

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
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y-0.2f), -Vector2.up, groundLayer);
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
        public void Hurt(float str)
        {
            anim.SetTrigger("hurt");
            if (direction == 1)
                rb.AddForce(new Vector2(-5f *str, 2f), ForceMode2D.Impulse);
            else
                rb.AddForce(new Vector2(5f * str, 2f), ForceMode2D.Impulse);
            
        }
        public void Die()
        {
            anim.SetTrigger("die");
            alive = false;
            gameObject.tag = "dead";
            StartCoroutine(LoadLevel(2));
            
        }
        void Restart()
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                anim.SetTrigger("idle");
                alive = true;
            }
        }
        void OnTriggerEnter2D(Collider2D collision) {
            if(collision.gameObject.tag == "NextScene") {
                StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex+1));
            }
        }
        IEnumerator LoadLevel(int levelIndex) {
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
}