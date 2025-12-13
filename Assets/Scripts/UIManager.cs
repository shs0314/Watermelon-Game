using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public TMP_Text scoreText;
    public TMP_Text bestScoreText;
    public TMP_Text subScoreText;
    public GameObject endGroup;
    public GameObject startGroup;

    public void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

}
