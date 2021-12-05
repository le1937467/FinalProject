using System.Collections;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(StartTimer());
    }
    IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(0.3f);
        Destroy(gameObject);
    }
}
