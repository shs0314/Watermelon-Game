using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public TMP_Text scoreText;
    public TMP_Text bestScoreText;
    public TMP_Text subScoreText;
    public GameObject startGroup;
    public GameObject endGroup;

    public void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void ShowScoreScreen()
    {
        scoreText.gameObject.SetActive(true);
        bestScoreText.gameObject.SetActive(true);
        startGroup.SetActive(false);
    }
}
