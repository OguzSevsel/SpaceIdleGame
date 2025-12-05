using System.Linq;
using UnityEngine;

public class EnemyShip : Ship
{
    private bool isInside = false;

    public override void Start()
    {
        base.Start();
        _cursorField = FindAnyObjectByType<CursorField>();
    }

    public void Update()
    {
        float dist = Vector2.Distance(transform.position, _cursorField.transform.position);

        if (!isInside && dist <= _cursorField.radius)
        {
            isInside = true;
            _cursorField.RegisterShip(this);
        }
        else if (isInside && dist > _cursorField.radius)
        {
            isInside = false;
            _cursorField.UnregisterShip(this);
        }
    }
}
