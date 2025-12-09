using System.Collections;
using UnityEngine;

public class ExplosionManager : MonoBehaviour
{
    public static ExplosionManager Instance;
    public GameObject explosionPrefab;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PoolManager.Instance.CreatePool(explosionPrefab, this.transform, 10);
    }

    public void CreateExplosion(Vector3 position)
    {
        var explosion = PoolManager.Instance.Get(this.gameObject);

        if (explosion != null)
        {
            explosion.transform.position = position;
            explosion.SetActive(true);
            var explosionPS = explosion.GetComponent<ParticleSystem>();
            explosionPS.Play();
            StartCoroutine(DeactivateExplosion(explosion));
        }
        else
        {
            explosion = Instantiate(explosionPrefab, this.transform);
            explosion.SetActive(false);
            PoolManager.Instance.AddToPool(this.gameObject, explosion);
            explosion.transform.position = position;
            explosion.SetActive(true);
            var explosionPS = explosion.GetComponent<ParticleSystem>();
            explosionPS.Play();
            StartCoroutine(DeactivateExplosion(explosion));
        }
    }

    private IEnumerator DeactivateExplosion(GameObject explosionObj)
    {
        while (explosionObj.GetComponent<ParticleSystem>().isPlaying)
        {
            yield return null;
        }

        explosionObj.SetActive(false);
    }
}
