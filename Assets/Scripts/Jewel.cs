using System.Collections;
using UnityEngine;

public class Jewel : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 90.0f;
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private float collectDistance = 1.25f;
    [SerializeField] private CanvasGroup victoryCanvasGroup;
    private Character player;
    private bool wasCollected;

    private void Awake()
    {
        this.EnsureTrigger();
        this.FindVictoryCanvas();
        this.HideVictoryScreen();
        this.player = FindFirstObjectByType<Character>();
    }

    private void Update()
    {
        this.transform.Rotate(Vector3.up, this.rotationSpeed * Time.deltaTime, Space.World);

        if (this.wasCollected || this.player == null)
        {
            return;
        }

        if (Vector3.Distance(this.transform.position, this.player.transform.position) <= this.collectDistance)
        {
            this.Collect();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (this.wasCollected || other.GetComponent<Character>() == null)
        {
            return;
        }

        this.Collect();
    }

    private void Collect()
    {
        this.wasCollected = true;
        this.StartCoroutine(this.ShowVictoryScreen());
    }

    private void EnsureTrigger()
    {
        Collider trigger = this.GetComponent<Collider>();
        if (trigger == null)
        {
            trigger = this.gameObject.AddComponent<BoxCollider>();
        }

        if (trigger is MeshCollider meshCollider)
        {
            meshCollider.convex = true;
        }

        trigger.isTrigger = true;

        BoxCollider boxTrigger = this.GetComponent<BoxCollider>();
        if (boxTrigger == null)
        {
            boxTrigger = this.gameObject.AddComponent<BoxCollider>();
        }

        boxTrigger.isTrigger = true;
        boxTrigger.center = new Vector3(0.0f, 0.45f, 0.0f);
        boxTrigger.size = new Vector3(1.5f, 1.8f, 1.5f);
    }

    private IEnumerator ShowVictoryScreen()
    {
        if (this.victoryCanvasGroup == null)
        {
            yield break;
        }

        this.victoryCanvasGroup.gameObject.SetActive(true);
        this.victoryCanvasGroup.alpha = 0.0f;

        float timer = 0.0f;
        while (timer < this.fadeDuration)
        {
            this.victoryCanvasGroup.alpha = timer / this.fadeDuration;
            timer += Time.deltaTime;
            yield return null;
        }

        this.victoryCanvasGroup.alpha = 1.0f;
        this.victoryCanvasGroup.interactable = true;
        this.victoryCanvasGroup.blocksRaycasts = true;
        this.gameObject.SetActive(false);
    }

    private void FindVictoryCanvas()
    {
        if (this.victoryCanvasGroup != null)
        {
            return;
        }

        GameObject victoryCanvas = GameObject.Find("VictoryCanvas");
        if (victoryCanvas != null)
        {
            this.victoryCanvasGroup = victoryCanvas.GetComponent<CanvasGroup>();
        }
    }

    private void HideVictoryScreen()
    {
        if (this.victoryCanvasGroup == null)
        {
            return;
        }

        this.victoryCanvasGroup.alpha = 0.0f;
        this.victoryCanvasGroup.interactable = false;
        this.victoryCanvasGroup.blocksRaycasts = false;
    }
}
