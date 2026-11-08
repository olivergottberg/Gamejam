using UnityEngine;

public class ShipPortal : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    private bool activated;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated)
            return;

        PlayerController player =
            other.GetComponentInParent<PlayerController>();

        if (player == null)
            return;

        activated = true;
        player.EnterShipMode();
        gameManager.PlayToxicTrack();
    }
}