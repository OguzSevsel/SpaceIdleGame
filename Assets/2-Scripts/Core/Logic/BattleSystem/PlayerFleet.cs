using System.Collections.Generic;
using UnityEngine;

public class PlayerFleet : MonoBehaviour
{
    [SerializeField] private Formation _formation;
    private GameObject _ship;
    [SerializeField] private GameObject _playerShipPrefab;
    [SerializeField] private int _spawnCount = 1;

    private void Start()
    {
        List<Vector3> spots = FormationGenerator.Generate(_formation.FormationShape, _spawnCount, _formation.FormationLayerCount, 0.5f);

        foreach (var pos in spots)
        {
            _ship = Instantiate(_playerShipPrefab, transform.position + pos, Quaternion.identity, transform);
        }
    }
}
