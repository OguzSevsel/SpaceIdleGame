using System;
using System.ComponentModel.Design;
using System.Linq;
using UnityEngine;

public class PlayerShip : Ship
{
    private void Awake()
    {
        CursorField.isClicked += HandleClick;
    }

    private void HandleClick(EnemyShip enemyShip)
    {
        Target = enemyShip;
        IsShooting = true;
    }
}
