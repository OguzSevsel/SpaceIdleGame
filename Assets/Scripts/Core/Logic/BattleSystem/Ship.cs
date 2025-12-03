using UnityEngine;

public class Ship : MonoBehaviour
{
    [SerializeField] private ShipSO shipSO;
    private int _currentHealth;
    

    private void Shoot()
    {
        
    }

    private void Move()
    {
        transform.position = new Vector2(transform.position.x, transform.position.y);
    }

    private void TakeDamage()
    {

    }
}
