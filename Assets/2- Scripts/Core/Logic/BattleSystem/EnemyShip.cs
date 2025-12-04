using System.Linq;
using UnityEngine;

public class EnemyShip : Ship
{
    private CursorField detector;
    private bool isInside = false;

    void Start()
    {
        detector = FindAnyObjectByType<CursorField>();
        base.Start();
    }

    void Update()
    {
        float dist = Vector2.Distance(transform.position, detector.transform.position);

        if (!isInside && dist <= detector.radius)
        {
            isInside = true;
            detector.RegisterShip(this);
        }
        else if (isInside && dist > detector.radius)
        {
            isInside = false;
            detector.UnregisterShip(this);
        }
    }
}
