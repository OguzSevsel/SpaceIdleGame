using UnityEngine;

public class BulletManager : MonoBehaviour
{
    public static BulletManager Instance;

    private void Awake() => Instance = this;

    public void Register(Bullet b) => ShipManager.Instance.Bullets.Add(b);

    private void Update()
    {
        foreach (var b in ShipManager.Instance.Bullets)
            if (b.IsActive) b.Tick();
    }
}
