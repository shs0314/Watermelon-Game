using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using UnityEngine.Pool;

public class EffectPool : MonoBehaviour
{
    public static EffectPool instance;

    private ObjectPool<ParticleSystem> pool;
    public GameObject particlePrefab;
    public Transform particleGroup;

    public int defaultCapacity;
    public int maxSize;

    public void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        pool = new ObjectPool<ParticleSystem>(CreateParticle,OnActivate,OnRelease,OnDelete,true,defaultCapacity,maxSize);
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
        GameObject instantParticle = Instantiate(particlePrefab, particleGroup);
        ParticleSystem particle = instantParticle.GetComponent<ParticleSystem>();
        return particle;
    }

    private void OnActivate(ParticleSystem particle)
    {
        particle.gameObject.SetActive(true);
    }

    private void OnRelease(ParticleSystem particle)
    {
        particle.gameObject.SetActive(false);
    }

    private void OnDelete(ParticleSystem particle)
    {
        Destroy(particle);
    }
    
}
