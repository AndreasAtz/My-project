using System;
using System.Collections;
using TMPro;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance => instance;
    private PlayerStatistics statistics;
    [SerializeField] private CanvasGroup hudCanvasGroup;
    [SerializeField] private CanvasGroup gameOverCanvasGroup;
    [SerializeField] private float fadingTime = 2.0f;
    private bool isFadingInGameOver = false;

    [SerializeField] private TextMeshProUGUI coinCounterText;
    [SerializeField] private Image healthBar;
    [SerializeField] private Character character;


    private IEnumerator FadeInGameOver()
    {
        this.isFadingInGameOver = true;
        float timer = 0.0f;
        while (timer < this.fadingTime)
        {
            float percent = timer / this.fadingTime;
            this.hudCanvasGroup.alpha = 1.0f - percent;
            this.gameOverCanvasGroup.alpha = percent;
            yield return null;
            timer += Time.deltaTime;
        }
        this.hudCanvasGroup.alpha = 0.0f;
        this.gameOverCanvasGroup.alpha = 0.0f;
    }
    private void Awake()
    {
        instance = this;
        this.statistics = new PlayerStatistics() { coinCounter = 0 };
    }

    private void Update()
    {
        float healthInPercent = this.character.GetCurrentHealth() / this.character.GetMaxHealth();
        this.healthBar.fillAmount = healthInPercent;

        if (healthInPercent <= 0.0f && !this.isFadingInGameOver)
        {
            this.StartCoroutine(FadeInGameOver());
        }
    }

    public void CollectCoin()
    {
        this.statistics.coinCounter++;
        string coinText = $"{this.statistics.coinCounter}";
        this.coinCounterText.text = coinText;
    }

    private class PlayerStatistics
    {
        public int coinCounter = 0;
    }
}
