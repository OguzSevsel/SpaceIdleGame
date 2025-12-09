using System.Collections.Generic;
using UnityEngine;

public class PlayerFleet : MonoBehaviour
{
    [SerializeField] private Formation _formation;
    private GameObject _ship;
    [SerializeField] private GameObject _playerDpsShipPrefab;
    [SerializeField] private GameObject _playerHealerShipPrefab;
    [SerializeField] private int _spawnCount = 1;

    private void Start()
    {

    }


    //private void FormationCreation()
    //{
    //    (Dictionary<List<Vector3>, int> positions, Dictionary<List<Quaternion>, int> rotations) formation = FormationGenerator.Generate(_formation.Shape, _spawnCount, _formation.LayerCount, 2f);

    //    int layer = 0;


    //    foreach (var layerPosition in formation.positions)
    //    {
    //        layer = layerPosition.Value;
    //        var ships = new List<GameObject>();

    //        foreach (var positions in layerPosition.Key)
    //        {
    //            Vector3 position = positions;

    //            if (layer == 1)
    //            {
    //                _ship = Instantiate(_playerHealerShipPrefab, transform.position + position, Quaternion.identity, transform);
    //                ships.Add(_ship);
    //            }
    //            else if (layer == 2)
    //            {
    //                _ship = Instantiate(_playerDpsShipPrefab, transform.position + position, Quaternion.identity, transform);
    //                ships.Add(_ship);
    //            }
    //            else if (layer == 3)
    //            {
    //                _ship = Instantiate(_playerDpsShipPrefab, transform.position + position, Quaternion.identity, transform);
    //                ships.Add(_ship);
    //            }
    //            else if (layer == 4)
    //            {
    //                _ship = Instantiate(_playerHealerShipPrefab, transform.position + position, Quaternion.identity, transform);
    //                ships.Add(_ship);
    //            }
    //            else if (layer == 5)
    //            {
    //                _ship = Instantiate(_playerDpsShipPrefab, transform.position + position, Quaternion.identity, transform);
    //                ships.Add(_ship);
    //            }
    //        }

    //        foreach (var layerRotation in formation.rotations)
    //        {
    //            int i = 0;

    //            foreach (var rotations in layerRotation.Key)
    //            {
    //                if (layer == layerRotation.Value)
    //                {
    //                    Quaternion rotation = rotations;
    //                    ships[i].transform.localRotation = rotation;
    //                    i++;
    //                }
    //            }
    //        }
    //    }
    //}
}
