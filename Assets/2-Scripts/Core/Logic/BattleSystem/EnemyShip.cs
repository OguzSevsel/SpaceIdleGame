using UnityEngine;

public class EnemyShip : Ship
{
    private bool isInside = false;

    public override void Start()
    {
        base.Start();
        _cursorField = FindAnyObjectByType<CursorField>();
    }

    public override void Tick()
    {
        base.Tick();
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

        if (CanAction)
        {
            Action();
        }
    }

    private void Action()
    {
        switch (DataSO.ShipType)
        {
            case ShipType.Dps:

                var target = GetTarget();
                if (target == null) return;
                Shoot(target);
                CanAction = false;
                ActionIntervalTimer = DataSO.ActionInterval;

                break;

            case ShipType.Tank:
                break;
            case ShipType.Healer:
                
                break;
            default:
                break;
        }
    }
}
