
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

                StartCoroutine(HealCoroutine());

                CanAction = false;
                ActionIntervalTimer = DataSO.ActionInterval;

                break;
            default:
                break;
        }
    }

    private IEnumerator HealCoroutine()
    {
        float duration = DataSO.ActionInterval;
        float timer = 0f;
        healthSpriteRenderer.enabled = true;

        while (timer <= duration)
        {
            timer += Time.deltaTime;
            Color color = healthSpriteRenderer.color;

            if (timer < duration / 2)
            {
                color.a = Mathf.Lerp(0f, 0.2f, timer / duration);
                healthSpriteRenderer.color = color;
            }
            
            if (timer >= duration / 2)
            {
                color.a = Mathf.Lerp(0.2f, 0f, timer / duration);
                healthSpriteRenderer.color = color;

                if (timer >= duration)
                {
                    Heal();
                    healthSpriteRenderer.enabled = false;
                    break;
                }
            }
            yield return null;
        }
    }
}
