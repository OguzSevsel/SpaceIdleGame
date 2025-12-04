//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//public class Bullet : MonoBehaviour
//{
//    public Ship TargetShip;
//    public float Speed;
//    public float Damage;
//    public bool IsActive;
//    public bool _isPlayerTargeted;
//    public Vector3 Direction;
//    public float Radius = 0.15f;
//    private Vector3 _lastPos;


//    private Vector3 _velocity;

//    public void Tick()
//    {
//        if (!IsActive) return;

//        // camera bounds check (keep after movement check if you prefer)
//        // Move with sweep
//        Vector3 newPos = transform.position + _velocity * Time.deltaTime;

//        // Sweep collision check (safe)
//        SweepCollision(_lastPos, newPos);

//        transform.position = newPos;
//        _lastPos = newPos;

//        // Re-check bounds after movement
//        
//    }
//    public void Shoot(Ship target, float damage)
//    {
//        gameObject.SetActive(true);

//        TargetShip = target;
//        Damage = damage;

//        _isPlayerTargeted = target is PlayerShip;

//        // If target is null or destroyed, fallback to default forward or deactivate
//        if (TargetShip == null)
//        {
//            // optionally set a default direction or just deactivate
//            Deactivate();
//            return;
//        }

//        Direction = (TargetShip.transform.position - transform.position).normalized;
//        _velocity = Direction * Speed;
//        IsActive = true;

//        // IMPORTANT: initialize lastPos to current position so sweep is valid
//        _lastPos = transform.position;
//    }

//    //public void Shoot(Ship target, float damage)
//    //{
//    //    this.gameObject.SetActive(true);
//    //    TargetShip = target;
//    //    Damage = damage;
//    //    _isPlayerTargeted = false;

//    //    if (TargetShip is PlayerShip)
//    //    {
//    //        _isPlayerTargeted = true;
//    //    }

//    //    Direction = (TargetShip.transform.position - transform.position).normalized;
//    //    _velocity = Direction * Speed;
//    //    IsActive = true;
//    //}
//    private void OnDrawGizmos()
//    {
//        Gizmos.color = Color.red;
//        Gizmos.DrawWireSphere(transform.position, Radius);
//    }

//    public void Deactivate()
//    {
//        IsActive = false;
//        this.gameObject.SetActive(false);
//    }

//    private void SweepCollision(Vector3 from, Vector3 to)
//    {
//        // If we have a valid target, try it first (guard against destroyed)
//        if (TargetShip != null)
//        {
//            if (TargetShip == null || TargetShip.gameObject == null || !TargetShip.gameObject.activeInHierarchy)
//            {
//                // target was destroyed — drop reference and fallback to general check
//                TargetShip = null;
//            }
//            else
//            {
//                float r = TargetShip.HitRadius + Radius;
//                if (LineIntersectsCircle(from, to, TargetShip.transform.position, r))
//                {
//                    TargetShip.TakeDamage(Damage);
//                    Deactivate();
//                    return;
//                }
//            }
//        }

//        // General collision check against opposite faction
//        var list = _isPlayerTargeted
//            ? ShipManager.Instance.GetShipsByType(returnEnemyShips: false) // check players
//            : ShipManager.Instance.GetShipsByType(returnEnemyShips: true); // check enemies

//        // Iterate safely, skip null/destroyed/inactive entries
//        for (int i = 0; i < list.Count; i++)
//        {
//            Ship ship = list[i];
//            if (ship == null) continue;
//            if (ship.gameObject == null) continue;
//            if (!ship.gameObject.activeInHierarchy) continue;

//            // If we still have an explicit target, skip other ships (optional)
//            // if (TargetShip != null && ship == TargetShip) continue;

//            float r = ship.HitRadius + Radius;
//            if (LineIntersectsCircle(from, to, ship.transform.position, r))
//            {
//                ship.TakeDamage(Damage);
//                Deactivate();
//                return;
//            }
//        }
//    }

//    // Robust line-vs-circle test
//    private bool LineIntersectsCircle(Vector3 a, Vector3 b, Vector3 center, float radius)
//    {
//        Vector3 ab = b - a;
//        float abLenSq = ab.sqrMagnitude;

//        // If the bullet didn't move this frame, just test point distance
//        if (abLenSq <= Mathf.Epsilon)
//            return (a - center).sqrMagnitude <= radius * radius;

//        // Project center onto line segment
//        float t = Vector3.Dot(center - a, ab) / abLenSq;
//        t = Mathf.Clamp01(t);

//        Vector3 closest = a + ab * t;
//        return (closest - center).sqrMagnitude <= radius * radius;
//    }

//    private void CheckGeneralCollision()
//    {
//        // If bullet is aimed at player -> it should check players
//        // If not -> it should check enemies
//        var list = _isPlayerTargeted
//            ? ShipManager.Instance.GetShipsByType(returnEnemyShips: false)
//            : ShipManager.Instance.GetShipsByType(returnEnemyShips: true);

//        for (int i = 0; i < list.Count; i++)
//        {
//            Ship ship = list[i];
//            if (ship == null) continue;

//            float r = ship.HitRadius + Radius;

//            if ((transform.position - ship.transform.position).sqrMagnitude <= r * r)
//            {
//                ship.TakeDamage(Damage);
//                Deactivate();
//                return;
//            }
//        }
//    }
//}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Ship TargetShip;
    public float Speed;
    public float Damage;
    public bool IsActive;
    public bool IsPlayerBullet;

    public float Radius = 0.15f;

    private Vector3 _velocity;
    private Vector3 _lastPos;

    public void Shoot(Ship target, float dmg, bool fromPlayer)
    {
        gameObject.SetActive(true);

        TargetShip = target;
        Damage = dmg;
        IsPlayerBullet = fromPlayer;

        Vector3 dir = (target.transform.position - transform.position).normalized;
        _velocity = dir * Speed;
        _lastPos = transform.position;

        IsActive = true;
    }

    public void Tick()
    {
        if (!IsActive) return;

        Vector3 newPos = transform.position + _velocity * Time.deltaTime;

        SweepCollision(_lastPos, newPos);

        transform.position = newPos;
        _lastPos = newPos;

        Vector3 p = transform.position;
        bool inside =
            p.x >= CameraBounds.Left &&
            p.x <= CameraBounds.Right &&
            p.y >= CameraBounds.Bottom &&
            p.y <= CameraBounds.Top;

        if (!inside)
        {
            Deactivate();
        }
    }

    private void SweepCollision(Vector3 from, Vector3 to)
    {
        List<Ship> nearby = SpatialGrid.Instance.GetNearbyShips(to);

        foreach (var ship in nearby)
        {
            if (ship == null) continue;

            // skip friendly
            if (IsPlayerBullet && ship is PlayerShip) continue;
            if (!IsPlayerBullet && ship is EnemyShip) continue;

            float R = ship.HitRadius + Radius;

            if (LineIntersectsCircle(from, to, ship.transform.position, R))
            {
                ship.TakeDamage(Damage);
                Deactivate();
                return;
            }
        }
    }

    private bool LineIntersectsCircle(Vector3 a, Vector3 b, Vector3 c, float r)
    {
        Vector3 ab = b - a;
        Vector3 ac = c - a;

        float t = Vector3.Dot(ac, ab.normalized);
        t = Mathf.Clamp(t, 0, ab.magnitude);

        Vector3 closest = a + ab.normalized * t;
        return (closest - c).sqrMagnitude <= r * r;
    }

    public void Deactivate()
    {
        IsActive = false;
        gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, Radius);
    }
}

