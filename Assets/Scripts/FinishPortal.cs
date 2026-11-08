using UnityEngine;

public class FinishPortal : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    private bool activated;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;

        if (other.GetComponentInParent<PlayerController>() == null)
            return;

        activated = true;
        gameManager.LevelWon();

        Debug.Log("Finish portal triggered");
    }
}