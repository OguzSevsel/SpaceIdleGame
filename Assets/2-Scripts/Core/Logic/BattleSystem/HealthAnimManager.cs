using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public class HealthAnim
{
    public Ship ship;
    public float timer;
    public float duration;
}

public class HealthAnimManager : MonoBehaviour
{
    private readonly List<HealthAnim> active = new List<HealthAnim>();
    public static HealthAnimManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void Add(Ship ship, float duration, float timer)
    {
        if (ship == null) return;
        if (ship.healthSpriteRenderer == null) return;

        // otherwise add new
        active.Add(new HealthAnim
        {
            ship = ship,
            duration = duration,
            timer = timer
        });
    }

    public void Remove(Ship ship)
    {
        for (int i = 0; i < active.Count; i++)
        {
            var item = active[i];
            if (item.ship == ship)
            {
                active.Remove(item);
            }
        }
    }

    private void Update()
    {
        for (int i = active.Count - 1; i >= 0; i--)
        {
            var anim = active[i];
            anim.timer += Time.deltaTime;

            float half = anim.duration * 0.5f;
            float peakAlpha = 0.7f;

            float alpha;

            if (anim.timer < half)
                alpha = Mathf.Lerp(0f, peakAlpha, anim.timer / half);
            else
                alpha = Mathf.Lerp(peakAlpha, 0f, (anim.timer - half) / half);

            var c = anim.ship.healthSpriteRenderer.color;
            c.a = alpha;
            anim.ship.healthSpriteRenderer.color = c;

            if (anim.timer >= anim.duration)
            {
                anim.timer = 0f;
                anim.ship.Heal();
                anim.ship.CanAction = false;
                anim.ship.ActionIntervalTimer = anim.ship.DataSO.ActionInterval;
            }
        }
    }
}

