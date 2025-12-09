using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFleet : MonoBehaviour
{
    private Formation _formation;
    private GameObject _ship;
    [SerializeField] private GameObject _playerDpsShipPrefab;
    [SerializeField] private GameObject _playerTankShipPrefab;
    [SerializeField] private GameObject _playerHealerShipPrefab;

    private void Start()
    {
        FormationCreationUI.OnFormationSaved += OnFormationSavedHandler;
        BattleSystemUI.OnStartLevel += OnStartLevelHandler;
    }

    private void OnStartLevelHandler()
    {
        if (this._formation == null)
        {
            Debug.Log("Create formation first");
            return;
        }

        var layers = _formation.GetLayers();

        foreach (var layer in layers)
        {
            int unitCount = layer.layerUnitCount;

            for (int i = 0; i < unitCount; i++)
            {
                SpawnShipByType(layer.ShipType, layer.positions[i], layer.rotations[i], this.gameObject.transform);
            }
        }
    }

    private void OnFormationSavedHandler(Formation formation)
    {
        this._formation = formation;
    }

    private void SpawnShipByType(ShipType shipType, Vector3 position, Quaternion rotation, Transform parent)
    {
        switch (shipType)
        {
            case ShipType.Dps:
                _ship = Instantiate(_playerDpsShipPrefab, position, rotation, parent);
                break;
            case ShipType.Tank:
                _ship = Instantiate(_playerTankShipPrefab, position, rotation, parent);
                break;
            case ShipType.Healer:
                _ship = Instantiate(_playerHealerShipPrefab, position, rotation, parent);
                break;
            default:
                break;
        }
    }
}
