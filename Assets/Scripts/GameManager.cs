using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Fruit lastFruit;
    public int score;
    public bool isOver;

    public readonly int FRUIT_MAX_LEVEL = 7;
    public int gameMaxLevel;
    public float lastX = 0f;

    public GameObject line;
    public GameObject bottom;
    public GameObject groundMask;
    public GameObject leftWall;
    public GameObject rightWall;
    public GameObject indicator;

    public void Awake()
    {
        Application.targetFrameRate = 60;
        UIManager.instance.bestScoreText.text = GetBestScore().ToString();
    }

    public void Start()
    {
        StartGame();
    }

    public void Update()
    {
        if(Input.GetButton("Cancel")) Application.Quit();
    }

    public void LateUpdate()
    {
        UIManager.instance.scoreText.text = score.ToString();
    }

    public void StartGame()
    {
        InitializeGame();
        SoundManager.instance.PlayBgm();
        Invoke("NextFruit", 1f);
    }

    public void FinishGame()
    {
        if(isOver) return;
        isOver = true;
        StartCoroutine(GameOverCoroutine());
    }

    public void Reset()
    {
        SoundManager.instance.PlaySfx(Sfx.Button);
        StartCoroutine(ResetCoroutine());
    }

    public void TouchUp()
    {
        if(lastFruit == null) return;
        lastX = lastFruit.transform.position.x;
        lastFruit.Drop();
        lastFruit = null;
        indicator.SetActive(false);
    }

    public void TouchDown()
    {
        if(lastFruit == null) return;
        lastFruit.Drag();
        indicator.SetActive(true);
    }

    public void NextFruit()
    {
        if(isOver) return;
        lastFruit = FruitPool.instance.Get();
        lastFruit.particle = EffectPool.instance.Get();
        lastFruit.transform.position = new Vector3(lastX,5.1f,0);
        StartCoroutine(WaitNextFruit());
    }

    private IEnumerator WaitNextFruit()
    {
        while(lastFruit != null) yield return null;
        yield return new WaitForSeconds(1f);
        NextFruit();
    }

    private void InitializeGame()
    {
        line.SetActive(true);
        bottom.SetActive(true);
        groundMask.SetActive(true);
        UIManager.instance.ShowScoreScreen();
        MoveWallToBackground();
        MoveBotoomToBackground();
    }

    private IEnumerator GameOverCoroutine()
    {
        yield return ClearAllFruits();
        SetBestScore(score);
        groundMask.SetActive(false);
        UIManager.instance.endGroup.SetActive(true);
        SoundManager.instance.StopBgm();
        SoundManager.instance.PlaySfx(Sfx.Finish);
    }

    private IEnumerator ClearAllFruits()
    {
        Fruit[] fruits = FindObjectsByType<Fruit>(FindObjectsSortMode.None);
        ParticleSystem[] particles = FindObjectsByType<ParticleSystem>(FindObjectsSortMode.None);

        foreach(Fruit fruit in fruits) fruit.rigidBody.simulated = false;
        foreach(Fruit fruit in fruits)
        {
            RemoveFruit(fruit);
            yield return new WaitForSeconds(0.1f);
        }
        foreach(ParticleSystem particle in particles) EffectPool.instance.Release(particle);
    }

    private IEnumerator ResetCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene("main");
    }

    public void RemoveFruit(Fruit fruit)
    {
        fruit.PlayParticle();
        fruit.Hide();
    }

    public int GetBestScore()
    {
        if(!PlayerPrefs.HasKey("BestScore")) PlayerPrefs.SetInt("BestScore", 0);
        return PlayerPrefs.GetInt("BestScore");
    }

    public void SetBestScore(int score)
    {
        int bestScore = Mathf.Max(score, GetBestScore());
        PlayerPrefs.SetInt("BestScore", bestScore);
        UIManager.instance.subScoreText.text = "점수 : " + score;
    }

    public void AddScoreByFruit(Fruit fruit)
    {
        int level = fruit.level;
        int point = (level + 1) * (level + 2) / 2;
        score += point;
    }

    public void MoveWallToBackground()
    {
        Vector3[] corners = UIManager.instance.GetBackgroundWorldCorners();
        float leftCornerX = corners[1].x;
        float rightCornerX = corners[2].x;
        leftWall.transform.position = new Vector3(leftCornerX - 0.35f, 0, 0);
        rightWall.transform.position = new Vector3(rightCornerX + 0.35f, 0, 0);
    }

    public void MoveBotoomToBackground()
    {
        float bottomCornerY = UIManager.instance.GetBackgroundWorldCorners()[0].y;
        bottom.transform.position = new Vector3(0,bottomCornerY -2.35f,0);
        groundMask.transform.position = new Vector3(0,bottomCornerY -2.6f,0);
    }
    
    public void MoveIndicator(float x)
    {
        indicator.transform.position = new Vector3(x, -1.68f, 0);
    }

}
