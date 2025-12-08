using System.Collections;
using UnityEngine;

public class ExplosionManager : MonoBehaviour
{
    public static ExplosionManager Instance;

    public GameObject explosionPrefab;
    public GameObject explosionInstance;


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
        explosionInstance = PoolManager.Instance.Get(this.gameObject);

        if (explosionInstance != null)
        {
            explosionInstance.transform.position = position;
            explosionInstance.SetActive(true);
            var explosionPS = explosionInstance.GetComponent<ParticleSystem>();
            explosionPS.Play();
            StartCoroutine(DeactivateExplosion(explosionInstance));
        }
        else
        {
            explosionInstance = Instantiate(explosionPrefab, this.transform);
            explosionInstance.SetActive(false);
            PoolManager.Instance.AddToPool(this.gameObject, explosionInstance);
            explosionInstance.transform.position = position;
            explosionInstance.SetActive(true);
            var explosionPS = explosionInstance.GetComponent<ParticleSystem>();
            explosionPS.Play();
            StartCoroutine(DeactivateExplosion(explosionInstance));
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
