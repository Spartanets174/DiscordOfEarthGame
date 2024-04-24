using System.Collections;
using UnityEngine;

public class EffectDestroyer : MonoBehaviour
{
    [SerializeField]
    private float delay;

    private void Start()
    {
        StartCoroutine(DestroyDelayed());
    }

    private void OnDestroy()
    {
        StopCoroutine(DestroyDelayed());
    }

    private IEnumerator DestroyDelayed()
    {
        yield return new WaitForSecondsRealtime(delay);

        Destroy(gameObject);
    }
}
