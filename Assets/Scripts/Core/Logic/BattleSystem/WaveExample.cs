using UnityEngine;

public class WaveExample : MonoBehaviour
{
    public FormationSpawner spawner;

    void Start()
    {
        spawner.SpawnFormation("v", 7);
        spawner.SpawnFormation("line", 10);
        spawner.SpawnFormation("circle", 12);
    }
}
