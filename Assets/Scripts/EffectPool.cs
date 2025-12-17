using UnityEngine;
using UnityEngine.Pool;

public class EffectPool : MonoBehaviour
{
    public static EffectPool instance;

    public GameObject particlePrefab;
    public Transform particleGroup;

    private ObjectPool<ParticleSystem> pool;

    public int defaultCapacity;
    public int maxSize;

    public void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        
        pool = new ObjectPool<ParticleSystem>(
            createFunc: CreateParticle,
            actionOnGet: OnGetParticle,
            actionOnRelease: OnReleaseParticle, 
            actionOnDestroy: OnDestroyParticle, 
            collectionCheck: true, 
            defaultCapacity: defaultCapacity, 
            maxSize: maxSize
        );
    }

    public ParticleSystem Get()
    {
        return pool.Get();
    }

    public void Release(ParticleSystem particle)
    {
        pool.Release(particle);
    }

    private ParticleSystem CreateParticle()
    {
        GameObject instantParitcle = Instantiate(particlePrefab, particleGroup);
        ParticleSystem particle = instantParitcle.GetComponent<ParticleSystem>();
        return particle;
    }

    private void OnGetParticle(ParticleSystem particle)
    {
        particle.gameObject.SetActive(true);
    }

    private void OnReleaseParticle(ParticleSystem particle)
    {
        particle.gameObject.SetActive(false);
    }

    private void OnDestroyParticle(ParticleSystem particle)
    {
        Destroy(particle);
    }

}
