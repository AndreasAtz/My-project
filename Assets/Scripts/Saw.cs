using UnityEngine;

public class Saw : MonoBehaviour
{
    [Header("Spinning")]
    [SerializeField] private float spinSpeed = 300f;
    [SerializeField] private Vector3 spinAxis = Vector3.forward;

    [Header("Audio")]
    
    [SerializeField] private AudioClip idleSound;
    [SerializeField] private AudioClip CuttingSound;

    [Header("Damage")]
    [SerializeField] private float damageAmount = 20.0f;
    [SerializeField] private float damageInterval = 0.75f;

    private AudioSource audioSource;
    private bool isCutting = false;
    private Character characterInSaw;
    private float damageTimer;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.loop = true;
            audioSource.playOnAwake = true;
        }
    }

    private void Start()
    {
        if(audioSource != null && idleSound != null)
        {
            audioSource.clip = idleSound;
            audioSource.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Character character = other.GetComponent<Character>();
        if (character != null)
        {
            characterInSaw = character;
            damageTimer = 0.0f;
            DamageCharacter();
            SetState(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Character character = other.GetComponent<Character>();
        if (character != null && character == characterInSaw)
        {
            characterInSaw = null;
            SetState(false);
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        Character character = other.GetComponent<Character>();
        if (character != null)
        {
            characterInSaw = character;
        }
    }

    private void SetState(bool newState)
    {
        if (isCutting == newState)
        return;

        if (newState)
        {
            isCutting = true;
            if (audioSource != null && CuttingSound != null)
            {
                audioSource.clip = CuttingSound;
                audioSource.Play();
            }
        } else
        {
            isCutting = false;
            if (audioSource != null && idleSound != null)
            {
                audioSource.clip = idleSound;
                audioSource.Play();
            }
        }
    }

    private void Update()
    {
        transform.Rotate(spinAxis, spinSpeed * Time.deltaTime);

        if (characterInSaw == null)
        {
            return;
        }

        damageTimer -= Time.deltaTime;
        if (damageTimer <= 0.0f)
        {
            DamageCharacter();
        }
    } 

    private void DamageCharacter()
    {
        if (characterInSaw == null)
        {
            return;
        }

        characterInSaw.InflictDamage(damageAmount);
        damageTimer = damageInterval;
    }
}
