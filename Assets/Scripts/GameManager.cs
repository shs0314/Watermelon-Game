using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Fruit lastFruit;
    public int score;
    public bool isOver;

    public readonly int FruitMaxLevel = 7;
    public int maxLevel;

    [Header("[ UI ]")]
    public TMP_Text scoreText;
    public TMP_Text bestScoreText;
    public TMP_Text subScoreText;
    public GameObject endGroup;
    public GameObject startGroup;

    [Header("[ ETC ]")]
    public GameObject line;
    public GameObject bottom;

    public void Awake()
    {
        Application.targetFrameRate = 60;
        bestScoreText.text = GetBestScore().ToString();
    }

    public void Update()
    {
        if (Input.GetButtonDown("Cancel")) Application.Quit();
    }

    public void LateUpdate()
    {
        scoreText.text = score.ToString();
    }

    public void StartGame()
    {
        InitializeGame();
        SoundManager.instance.PlaySfx(Sfx.Button);
        SoundManager.instance.PlayBgm();
        Invoke("NextFruit", 1f);
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
        while (lastFruit != null) yield return null;
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
        ParticleSystem[] particles = FindObjectsOfType<ParticleSystem>();

        foreach (Fruit fruit in fruits) fruit.rigidBody.simulated = false;

        foreach (Fruit fruit in fruits)
        {
            DeleteFruit(fruit);
            yield return new WaitForSeconds(0.1f);
        }

        foreach(ParticleSystem particle in particles) EffectPool.instance.Release(particle);

        yield return new WaitForSeconds(0.1f);

        SetBestScore(score);
        endGroup.SetActive(true);
        SoundManager.instance.StopBgm();
        SoundManager.instance.PlaySfx(Sfx.Finish);
    }

    public void Reset()
    {
        SoundManager.instance.PlaySfx(Sfx.Button);
        StartCoroutine(ResetCoroutine());
    }

    IEnumerator ResetCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene("Main");
    }

    public int GetBestScore()
    {
        if (!PlayerPrefs.HasKey("BestScore")) PlayerPrefs.SetInt("BestScore", 0);
        return PlayerPrefs.GetInt("BestScore");
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

    public void SetBestScore(int score)
    {
        int bestScore = Mathf.Max(score, GetBestScore());
        PlayerPrefs.SetInt("BestScore", bestScore);
        subScoreText.text = "점수 : " + scoreText.text;
    }

    public void AddScoreByFruit(Fruit fruit)
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

    private void InitializeGame()
    {
        line.SetActive(true);
        bottom.SetActive(true);
        scoreText.gameObject.SetActive(true);
        bestScoreText.gameObject.SetActive(true);
        startGroup.SetActive(false);
    }
    
}