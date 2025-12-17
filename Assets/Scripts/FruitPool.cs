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
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        
        pool = new ObjectPool<Fruit>(
            createFunc: CreateFruit,
            actionOnGet: OnGetFruit,
            actionOnRelease: OnReleaseFruit, 
            actionOnDestroy: OnDestroyFruit, 
            collectionCheck: true, 
            defaultCapacity: defaultCapacity, 
            maxSize: maxSize
        );
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
        GameObject instantFruit = Instantiate(fruitPrefab, fruitGroup);
        Fruit fruit = instantFruit.gameObject.GetComponent<Fruit>();
        fruit.gameManager = gameManager;
        return fruit;
    }

    private void OnGetFruit(Fruit fruit)
    {
        fruit.level = Random.Range(0,4);
        fruit.gameObject.SetActive(true);
    }

    private void OnReleaseFruit(Fruit fruit)
    {
        fruit.gameObject.SetActive(false);
        fruit.Initialize();
    }

    private void OnDestroyFruit(Fruit fruit)
    {
        Destroy(fruit);
    }

}
