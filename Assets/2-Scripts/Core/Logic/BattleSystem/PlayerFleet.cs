using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.PlayerSettings;

public class PlayerFleet : MonoBehaviour
{
    [SerializeField] private Formation _formation;
    private GameObject _ship;
    [SerializeField] private GameObject _playerDpsShipPrefab;
    [SerializeField] private GameObject _playerHealerShipPrefab;
    [SerializeField] private int _spawnCount = 1;

    private void Start()
    {
        List<Vector3> spots = FormationGenerator.Generate(_formation.FormationShape, _spawnCount, _formation.FormationLayerCount, 2f);
        
        for (int i = 0; i < spots.Count; i++)
        {
            var pos = spots[i];

            if (i % 10 == 0)
            {
                _ship = Instantiate(_playerHealerShipPrefab, transform.position + pos, Quaternion.identity, transform);
            }
            else
            {
                _ship = Instantiate(_playerDpsShipPrefab, transform.position + pos, Quaternion.identity, transform);
            }
        }
    }
}
