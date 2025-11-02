using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class FruitPool : MonoBehaviour
{
    public static FruitPool instance;

    public GameObject fruitPrefab;
    public Transform fruitGroup;
    public GameManager gameManager;

    private ObjectPool<Fruit> pool;

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
        pool = new ObjectPool<Fruit>(CreateFruit, OnActivate, OnRelease, OnDelete, true, defaultCapacity, maxSize);
    }

    public Fruit Get()
    {
        return pool.Get();
    }

    public void Release(Fruit fruit)
    {
        pool.Release(fruit);
    }

    private Fruit CreateFruit()
    {
        GameObject fruitInstant = Instantiate(fruitPrefab, fruitGroup);
        Fruit fruit = fruitInstant.gameObject.GetComponent<Fruit>();
        fruit.gameManager = gameManager;
        return fruit;
    }

    private void OnActivate(Fruit fruit)
    {
        fruit.level = Random.Range(0, 4);
        fruit.gameObject.SetActive(true);
    }

    private void OnRelease(Fruit fruit)
    {
        fruit.gameObject.SetActive(false);
        fruit.Initialize();
    }

    private void OnDelete(Fruit fruit)
    {
        Destroy(fruit);
    }

}
