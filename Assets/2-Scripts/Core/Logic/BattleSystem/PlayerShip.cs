
using System.Collections.Generic;
using UnityEngine;

public class PlayerShip : Ship
{
    public override void Start()
    {
        base.Start();
        CursorField.isClicked += HandleClick;
    }

    private void HandleClick(List<EnemyShip> enemyShip)
    {
        int index = Random.Range(0, enemyShip.Count);
        if (enemyShip.Count == 0)
        {
            return;
        }
        Target = enemyShip[index];
        IsShooting = true;
    }
}
