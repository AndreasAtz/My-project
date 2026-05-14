using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Character character = other.gameObject.GetComponent<Character>();
        
        if (character != null)
        {
            character.SetHealthToZero();
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowGameOver();
            }
        }
    }
}
