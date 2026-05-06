using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] private Vector3 pointA;
    [SerializeField] private Vector3 pointB;
    [SerializeField] private AudioClip squashClip;

    private AudioSource audioSource;
    private bool isDead = false;
    private Vector3 currentTarget;

    void Start()
    {
        this.audioSource = this.GetComponent<AudioSource>();
        this.currentTarget = this.pointB;
    }

    void Update()
    {
        if (this.isDead) return;

        // Bewegen zwischen zwei Punkten
        this.transform.position = Vector3.MoveTowards(
            this.transform.position,
            this.currentTarget,
            this.moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(this.transform.position, this.currentTarget) < 0.1f)
        {
            this.currentTarget = this.currentTarget == this.pointA ? this.pointB : this.pointA;
            this.transform.forward = (this.currentTarget - this.transform.position).normalized;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Character"))
        {
            Character character = other.GetComponent<Character>();
            if (character != null)
            {
                this.Squash();
            }
        }
    }

    void Squash()
    {
        this.isDead = true;
        this.audioSource.PlayOneShot(this.squashClip);
        StartCoroutine(SquashAnimation());
    }

    System.Collections.IEnumerator SquashAnimation()
    {
        float duration = 0.3f;
        float timer = 0.0f;
        Vector3 originalScale = this.transform.localScale;
        Vector3 squashedScale = new Vector3(originalScale.x * 2f, originalScale.y * 0.1f, originalScale.z * 2f);

        while (timer < duration)
        {
            this.transform.localScale = Vector3.Lerp(originalScale, squashedScale, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(this.gameObject, 0.5f);
    }
}
