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
        (Dictionary<List<Vector3>, int>, Dictionary<List<Quaternion>, int>) formation = FormationGenerator.Generate(_formation.FormationShape, _spawnCount, _formation.FormationLayerCount, 2f);

        int layer = 0;


        foreach (var layerPosition in formation.Item1)
        {
            layer = layerPosition.Value;
            var ships = new List<GameObject>();

            foreach (var positions in layerPosition.Key)
            {
                Vector3 position = positions;

                if (layer == 1)
                {
                    _ship = Instantiate(_playerHealerShipPrefab, transform.position + position, Quaternion.identity, transform);
                    ships.Add(_ship);
                }
                else if (layer == 2)
                {
                    _ship = Instantiate(_playerDpsShipPrefab, transform.position + position, Quaternion.identity, transform);
                    ships.Add(_ship);
                }
                else if (layer == 3)
                {
                    _ship = Instantiate(_playerDpsShipPrefab, transform.position + position, Quaternion.identity, transform);
                    ships.Add(_ship);
                }
                else if (layer == 4)
                {
                    _ship = Instantiate(_playerHealerShipPrefab, transform.position + position, Quaternion.identity, transform);
                    ships.Add(_ship);
                }
                else if (layer == 5)
                {
                    _ship = Instantiate(_playerDpsShipPrefab, transform.position + position, Quaternion.identity, transform);
                    ships.Add(_ship);
                }
            }

            foreach (var layerRotation in formation.Item2)
            {
                int i = 0;

                foreach (var rotations in layerRotation.Key)
                {
                    if (layer == layerRotation.Value)
                    {
                        Quaternion rotation = rotations;
                        ships[i].transform.localRotation = rotation;
                        i++;
                    }
                }
            }
        }
    }
}
