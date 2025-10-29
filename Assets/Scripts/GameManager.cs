using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Fruit lastFruit;
    public GameObject fruitPrefab;
    public Transform fruitGroup;

    public GameObject particlePrefab;
    public Transform particleGroup;

    public readonly int FruitMaxLevel = 7;
    public int maxLevel;

    void Awake()
    {
        //프레임 고정
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        NextFruit();
    }

    Fruit GetFruit()
    {
        GameObject instantParticleObject = Instantiate(particlePrefab, particleGroup);
        ParticleSystem instantParitlce = instantParticleObject.GetComponent<ParticleSystem>();

        GameObject instantFruitObject = Instantiate(fruitPrefab, fruitGroup);
        Fruit instantFruit = instantFruitObject.GetComponent<Fruit>();

        instantFruit.particle = instantParitlce;

        return instantFruit;
    }
    
    void NextFruit()
    {
        Fruit newFruit = GetFruit();
        lastFruit = newFruit;
        lastFruit.gameManager = this;
        lastFruit.level = Random.Range(0, 4);
        lastFruit.gameObject.SetActive(true);
        StartCoroutine(WaitNextFruit());
    }

    IEnumerator WaitNextFruit()
    {
        while(lastFruit != null)
        {
            yield return null;
        }
        yield return new WaitForSeconds(2.5f);
        NextFruit();
    }

    public void TouchDown()
    {
        if (lastFruit == null) return;
        lastFruit.Drag();
    }

    public void TouchUp()
    {
        if (lastFruit == null) return;
        lastFruit.Drop();
        lastFruit = null;
    }
    
}
