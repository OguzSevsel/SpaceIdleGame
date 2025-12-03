using UnityEngine;
using System.Collections.Generic;

public class FormationController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float stopDistance = 7f; // Distance from player center

    private List<Transform> ships = new List<Transform>();
    private List<Vector3> offsets = new List<Vector3>();
    public Transform playerCenter;

    public void SetupFormation(List<Transform> shipList, Formation formation)
    {
        ships = shipList;
        offsets = formation.offsets;
    }

    void Update()
    {
        float dist = Vector3.Distance(transform.position, playerCenter.position);

        // Only move if outside the ring
        if (dist > stopDistance)
        {
            Vector3 dir = (playerCenter.position - transform.position).normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;

            Debug.Log($"TransformPos = {transform.position}" +
                $"\n" +
                $"Dir = {dir}" +
                $"\n" +
                $"Time.Deltatime = {Time.deltaTime}"
                );
        }

        // Keep ships in formation
        for (int i = 0; i < ships.Count; i++)
        {
            Vector3 target = transform.position + offsets[i];
            ships[i].position = Vector3.Lerp(ships[i].position, target, 8f * Time.deltaTime);
        }
    }
}
