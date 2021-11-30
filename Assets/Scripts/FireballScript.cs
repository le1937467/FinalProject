using ClearSky;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballScript : MonoBehaviour
{
    [SerializeField] private Animator playerAnimator;
    private SimplePlayerController player;

    private void Start()
    {
        player = GetComponentInParent<SimplePlayerController>();
        StartCoroutine(StartTimer());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy" && !collision.isTrigger)
        {
            EnemyUniversalController enemy = collision.GetComponent<EnemyUniversalController>();
            if (enemy)
            {
                PlayerCombat.GetInstance().DealDamageTo(enemy);
                Destroy(gameObject);
            }
        }
        else if (collision.tag != "Player" && collision.tag != "PlayerBody" && collision.isTrigger == false)
        {
            Destroy(gameObject);
        }
    }


    IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(4f);
        Destroy(gameObject);
    }

    
}
