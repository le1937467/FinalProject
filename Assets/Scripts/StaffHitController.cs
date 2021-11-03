using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffHitController : MonoBehaviour
{
    [SerializeField] private Animator playerAnimator;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Enemy" && !collision.isTrigger &&
            playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack") &&
            playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            EnemyUniversalController enemy = collision.GetComponent<EnemyUniversalController>();
            if(enemy)
                PlayerCombat.GetInstance().DealDamageTo(enemy);
        }
    }
}
