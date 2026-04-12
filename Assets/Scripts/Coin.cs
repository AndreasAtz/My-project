using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private AudioClip coinSound;
    [SerializeField] private AudioSource coinCollectAudioSource;
    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        return;

        coinCollectAudioSource.Play();

        Destroy(gameObject);
    }
}
