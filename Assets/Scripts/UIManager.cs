using System;
using System.Collections;
using TMPro;
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
    [SerializeField] private Transform respawnPoint;
    private Coroutine fadeRoutine;
    private bool isGameOverVisible = false;

    [SerializeField] private TextMeshProUGUI coinCounterText;
    [SerializeField] private Image healthBar;
    [SerializeField] private Character character;


    private IEnumerator FadeGameOver(bool showGameOver)
    {
        this.isGameOverVisible = showGameOver;
        float timer = 0.0f;
        float startHudAlpha = this.hudCanvasGroup.alpha;
        float startGameOverAlpha = this.gameOverCanvasGroup.alpha;
        float targetHudAlpha = showGameOver ? 0.0f : 1.0f;
        float targetGameOverAlpha = showGameOver ? 1.0f : 0.0f;

        this.SetCanvasInteraction(this.gameOverCanvasGroup, false);
        this.SetCanvasInteraction(this.hudCanvasGroup, false);

        while (timer < this.fadingTime)
        {
            float percent = timer / this.fadingTime;
            this.hudCanvasGroup.alpha = Mathf.Lerp(startHudAlpha, targetHudAlpha, percent);
            this.gameOverCanvasGroup.alpha = Mathf.Lerp(startGameOverAlpha, targetGameOverAlpha, percent);
            yield return null;
            timer += Time.deltaTime;
        }

        this.hudCanvasGroup.alpha = targetHudAlpha;
        this.gameOverCanvasGroup.alpha = targetGameOverAlpha;
        this.SetCanvasInteraction(this.gameOverCanvasGroup, showGameOver);
        this.SetCanvasInteraction(this.hudCanvasGroup, !showGameOver);
        this.fadeRoutine = null;
    }

    private void Awake()
    {
        instance = this;
        this.statistics = new PlayerStatistics() { coinCounter = 0 };
        this.FindMissingReferences();
        this.EnsureGameOverButtons();
        this.SetCanvasInteraction(this.gameOverCanvasGroup, false);
        this.SetCanvasInteraction(this.hudCanvasGroup, true);
        if (this.gameOverCanvasGroup != null)
        {
            this.gameOverCanvasGroup.alpha = 0.0f;
        }

        if (this.hudCanvasGroup != null)
        {
            this.hudCanvasGroup.alpha = 1.0f;
        }
        this.UpdateCoinCounter();
    }

    private void Update()
    {
        if (this.character == null || this.healthBar == null)
        {
            return;
        }

        float healthInPercent = this.character.GetCurrentHealth() / this.character.GetMaxHealth();
        this.healthBar.fillAmount = healthInPercent;

        if (healthInPercent <= 0.0f && !this.isGameOverVisible)
        {
            this.ShowGameOver();
        }
    }

    public void CollectCoin()
    {
        this.statistics.coinCounter++;
        this.UpdateCoinCounter();
    }

    public void ShowGameOver()
    {
        if (this.gameOverCanvasGroup == null || this.hudCanvasGroup == null)
        {
            return;
        }

        this.StartFade(true);
    }

    public void RespawnPlayer()
    {
        if (this.character == null)
        {
            return;
        }

        this.character.RespawnAt(this.respawnPoint);
        this.statistics.coinCounter = 0;
        this.UpdateCoinCounter();
        if (this.healthBar != null)
        {
            this.healthBar.fillAmount = 1.0f;
        }
        this.StartFade(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private void StartFade(bool showGameOver)
    {
        if (this.fadeRoutine != null)
        {
            this.StopCoroutine(this.fadeRoutine);
        }

        this.fadeRoutine = this.StartCoroutine(this.FadeGameOver(showGameOver));
    }

    private void UpdateCoinCounter()
    {
        if (this.coinCounterText != null)
        {
            this.coinCounterText.text = $"{this.statistics.coinCounter}";
        }
    }

    private void SetCanvasInteraction(CanvasGroup canvasGroup, bool enabled)
    {
        if (canvasGroup == null)
        {
            return;
        }

        canvasGroup.interactable = enabled;
        canvasGroup.blocksRaycasts = enabled;
    }

    private void FindMissingReferences()
    {
        if (this.hudCanvasGroup == null)
        {
            GameObject hudCanvas = GameObject.Find("HUDCanvas");
            if (hudCanvas != null)
            {
                this.hudCanvasGroup = hudCanvas.GetComponent<CanvasGroup>();
            }
        }

        if (this.gameOverCanvasGroup == null)
        {
            GameObject gameOverCanvas = GameObject.Find("GameOverCanvas");
            if (gameOverCanvas != null)
            {
                this.gameOverCanvasGroup = gameOverCanvas.GetComponent<CanvasGroup>();
            }
        }

        if (this.character == null)
        {
            this.character = FindFirstObjectByType<Character>();
        }

        if (this.respawnPoint == null)
        {
            GameObject respawnObject = GameObject.Find("Respawn_Point");
            if (respawnObject != null)
            {
                this.respawnPoint = respawnObject.transform;
            }
        }

        if (this.healthBar == null)
        {
            GameObject healthBarObject = GameObject.Find("HealthBar");
            if (healthBarObject != null)
            {
                this.healthBar = healthBarObject.GetComponent<Image>();
            }
        }
    }

    private void EnsureGameOverButtons()
    {
        if (this.gameOverCanvasGroup == null)
        {
            return;
        }

        if (this.gameOverCanvasGroup.transform.Find("RespawnButton") == null)
        {
            this.CreateButton("RespawnButton", "Respawn", new Vector2(-140.0f, -130.0f), this.RespawnPlayer);
        }

        if (this.gameOverCanvasGroup.transform.Find("ExitButton") == null)
        {
            this.CreateButton("ExitButton", "Exit", new Vector2(140.0f, -130.0f), this.ExitGame);
        }
    }

    private void CreateButton(string objectName, string label, Vector2 position, UnityEngine.Events.UnityAction onClick)
    {
        GameObject buttonObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(this.gameOverCanvasGroup.transform, false);

        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.sizeDelta = new Vector2(240.0f, 64.0f);
        buttonRect.anchoredPosition = position;

        Image buttonImage = buttonObject.GetComponent<Image>();
        buttonImage.color = new Color(0.12f, 0.12f, 0.12f, 0.9f);

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = buttonImage;
        button.onClick.AddListener(onClick);

        GameObject textObject = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(buttonObject.transform, false);

        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        text.text = label;
        text.fontSize = 28.0f;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
    }

    private class PlayerStatistics
    {
        public int coinCounter = 0;
    }
}
