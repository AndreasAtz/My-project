using Unity.VisualScripting;
using UnityEngine;

public class Saw : MonoBehaviour
{
    [Header("Spinning")]
    [SerializeField] private float spinSpeed = 300f;
    [SerializeField] private Vector3 spinAxis = Vector3.forward;

    [Header("Audio")]
    
    [SerializeField] private AudioClip idleSound;
    [SerializeField] private AudioClip CuttingSound;

    [Header("Particles")]
    private AudioSource audioSource;
    private bool isCutting = false;

    


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = true;
    }

    private void Start()
    {
        if(idleSound != null)
        {
            audioSource.clip = idleSound;
            audioSource.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SetState(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SetState(false);
        }
    }

    private void SetState(bool newState)
    {
        if (isCutting == newState)
        return;

        if (newState)
        {
            isCutting = true;
            audioSource.clip = CuttingSound;
            audioSource.Play();
        } else
        {
            isCutting = false;
            audioSource.clip = idleSound;
            audioSource.Play();
        }
    }

    private void Update()
    {
        transform.Rotate(spinAxis, spinSpeed * Time.deltaTime);
    } 
}
