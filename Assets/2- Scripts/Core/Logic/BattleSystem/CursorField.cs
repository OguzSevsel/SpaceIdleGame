using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorField : MonoBehaviour
{
    public float radius = 1f;
    public LayerMask shipLayer;
    public static event Action<EnemyShip> isClicked;

    private void Update()
    {
        Vector2 screenPos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        worldPos.z = 0f;

        transform.position = worldPos;

        // --- PLAYER CLICK ---
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            DetectShipsInside();
        }
    }

    private void DetectShipsInside()
    {
        // find ships inside the circle
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, shipLayer);

        if (hits.Length == 0)
        {
            Debug.Log("No ships inside circle.");
            return;
        }

        foreach (Collider2D hit in hits)
        {
            EnemyShip ship = hit.GetComponent<EnemyShip>();
            if (ship != null)
            {
                Debug.Log("Ship inside circle: " + ship.name);
                isClicked.Invoke(ship);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
