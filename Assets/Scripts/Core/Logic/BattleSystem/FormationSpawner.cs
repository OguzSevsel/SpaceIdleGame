using System.Collections.Generic;
using UnityEngine;

public class FormationSpawner : MonoBehaviour
{
    public Camera cam;
    public GameObject enemyPrefab;
    public Transform playerFleetCenter;

    public float spawnOffset = 4f; // outside the screen

    public void SpawnFormation(string type, int count)
    {
        Formation formation;

        switch (type)
        {
            case "line": formation = FormationGenerator.Line(count, 0.7f); break;
            case "v": formation = FormationGenerator.VShape(count, 0.7f); break;
            case "circle": formation = FormationGenerator.Circle(count, 1.3f); break;
            default: formation = FormationGenerator.Box(3, 3, 0.7f); break;
        }

        int side = Random.Range(0, 4);

        Vector3 spawnPos;

        switch (side)
        {
            case 0: spawnPos = cam.ViewportToWorldPoint(new Vector3(0.5f, 1f, 10f)) + Vector3.up * spawnOffset; break;
            case 1: spawnPos = cam.ViewportToWorldPoint(new Vector3(0.5f, 0f, 10f)) - Vector3.up * spawnOffset; break;
            case 2: spawnPos = cam.ViewportToWorldPoint(new Vector3(0f, 0.5f, 10f)) - Vector3.right * spawnOffset; break;
            default: spawnPos = cam.ViewportToWorldPoint(new Vector3(1f, 0.5f, 10f)) + Vector3.right * spawnOffset; break;
        }

        GameObject center = new GameObject("FormationCenter");
        center.transform.position = spawnPos;

        var fc = center.AddComponent<FormationController>();
        fc.playerCenter = playerFleetCenter;
        fc.stopDistance = 7f; // distance from player

        // Spawn ships
        List<Transform> shipList = new List<Transform>();
        for (int i = 0; i < formation.offsets.Count; i++)
        {
            GameObject e = Instantiate(enemyPrefab);
            shipList.Add(e.transform);
        }

        fc.SetupFormation(shipList, formation);
    }
}
