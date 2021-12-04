using ClearSky;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffHitController : MonoBehaviour
{
    [SerializeField] private Animator playerAnimator;
    private PlayerController player;

    private void Start()
    {
        player = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy" && !collision.isTrigger && player.canHit)
        {
            EnemyUniversalController enemy = collision.GetComponent<EnemyUniversalController>();
            if (enemy)
            {
                player.canHit = false;
                PlayerCombat.GetInstance().DealDamageTo(enemy);
            }
        }
    }
}
