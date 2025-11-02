using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Fruit lastFruit;
    public int score;
    public bool isOver;

    public readonly int FruitMaxLevel = 7;
    public int maxLevel;

    void Awake()
    {
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        SoundManager.instance.PlayBgm();
        NextFruit();
    }
    
    void NextFruit()
    {
        if (isOver) return;
        
        lastFruit = FruitPool.instance.Get();
        lastFruit.particle = EffectPool.instance.Get();

        StartCoroutine(WaitNextFruit());
    }

    IEnumerator WaitNextFruit()
    {
        while(lastFruit != null)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        NextFruit();
    }

    public void FinishGame()
    {
        if (isOver) return;
        isOver = true;
        StartCoroutine(GameOverCoroutine());
    }
    
    IEnumerator GameOverCoroutine()
    {
        Fruit[] fruits = FindObjectsOfType<Fruit>();

        foreach (Fruit fruit in fruits) fruit.rigidBody.simulated = false;

        foreach (Fruit fruit in fruits)
        {
            DeleteFruit(fruit);
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.1f);
        SoundManager.instance.PlaySfx(SoundManager.Sfx.Finish);
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

    public void addScoreByFruit(Fruit fruit)
    {
        int level = fruit.level;
        int point = (level + 1) * (level + 2) / 2;
        score += point;
    }

    public void DeleteFruit(Fruit fruit)
    {
        fruit.PlayParticle();
        fruit.Hide();
    }
    
}
