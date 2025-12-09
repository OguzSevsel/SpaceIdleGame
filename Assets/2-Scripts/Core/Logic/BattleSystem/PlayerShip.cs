using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShip : Ship
{
    private bool _isShooting = false;

    public override void Start()
    {
        base.Start();
        CursorField.isClicked += HandleClick;
    }

    public override void Tick()
    {
        base.Tick();

        if (CanAction)
        {
            Action();
        }
    }

    private void HandleClick(List<EnemyShip> enemyShip)
    {
        if (DataSO.ShipType == ShipType.Dps)
        {
            int index = Random.Range(0, enemyShip.Count);
            if (enemyShip.Count == 0)
            {
                return;
            }
            Target = enemyShip[index];
            if (CanAction)
            {
                _isShooting = true;
            }
        }
    }

    private void Action()
    {
        switch (DataSO.ShipType)
        {
            case ShipType.Dps:
                
                if (!_isShooting) return;
                if (Target == null) return;
                Shoot(Target);
                CanAction = false;
                _isShooting = false;
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
