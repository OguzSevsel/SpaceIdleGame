using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CursorField : MonoBehaviour
{
    public float radius = 1f;
    public LayerMask shipLayer;
    public static event Action<List<EnemyShip>> isClicked;
    public List<EnemyShip> shipsInside = new List<EnemyShip>();

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
        if (shipsInside != null)
        {
            isClicked?.Invoke(shipsInside);
        }
    }

    public void RegisterShip(EnemyShip ship)
    {
        if (!shipsInside.Contains(ship))
        {
            shipsInside.Add(ship);
        }
    }

    public void UnregisterShip(EnemyShip ship)
    {
        if (shipsInside.Contains(ship))
        {
            shipsInside.Remove(ship);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
