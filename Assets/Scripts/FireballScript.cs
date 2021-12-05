using ClearSky;
using System.Collections;
using UnityEngine;

public class FireballScript : MonoBehaviour
{
    public GameObject impact;

    private void Start()
    {
        StartCoroutine(StartTimer());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy" && !collision.isTrigger)
        {
            EnemyUniversalController enemy = collision.GetComponent<EnemyUniversalController>();
            if (enemy)
            {
                Instantiate(impact, transform.position, Quaternion.identity);
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
