using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class EnemySpawner : MonoBehaviour
{
    private Dictionary<Transform, Transform> _spawnPoints;

    [SerializeField] private Transform _leftSpawnLocation;
    [SerializeField] private Transform _leftDestinationLocation;
    [SerializeField] private Formation _leftFormation;

    [SerializeField] private Transform _rightSpawnLocation;
    [SerializeField] private Transform _rightDestinationLocation;
    [SerializeField] private Formation _rightFormation;

    [SerializeField] private Transform _topSpawnLocation;
    [SerializeField] private Transform _topDestinationLocation;
    [SerializeField] private Formation _topFormation;

    [SerializeField] private Transform _bottomSpawnLocation;
    [SerializeField] private Transform _bottomDestinationLocation;
    [SerializeField] private Formation _bottomFormation;

    [SerializeField] private Transform _playerFleetLocation;
    [SerializeField] private GameObject _enemyShipPrefab;
    [SerializeField] private int _spawnCount = 1;

    private GameObject _ship;
    private bool _isSpawned = false;
    private float lerp = 0.01f;

    //This logic can be moved to a Coroutine for better performance
    private void Update()
    {
        if (_isSpawned)
        {
            foreach (KeyValuePair<Transform, Transform> pair in _spawnPoints)
            {
                MoveToDestination(pair.Key, pair.Value);
            }
        }
    }

    private void Start()
    {
        PopulateSpawnPoints();
        foreach (KeyValuePair<Transform, Transform> pair in _spawnPoints)
        {
            float spacing = 2f;

            (Dictionary<List<Vector3>, int>, Dictionary<List<Quaternion>, int>) formation = FormationGenerator.Generate(_leftFormation.FormationShape, _spawnCount, _leftFormation.FormationLayerCount, 2f);
            SpawnShips(formation, pair.Key);

            if (pair.Key == _rightSpawnLocation)
            {
                formation = FormationGenerator.Generate(_rightFormation.FormationShape, _spawnCount, _rightFormation.FormationLayerCount, spacing);
                SpawnShips(formation, pair.Key);
            }

            if (pair.Key == _topSpawnLocation)
            {
                formation = FormationGenerator.Generate(_topFormation.FormationShape, _spawnCount, _topFormation.FormationLayerCount, spacing);
                SpawnShips(formation, pair.Key);
            }

            if (pair.Key == _bottomSpawnLocation)
            {
                formation = FormationGenerator.Generate(_bottomFormation.FormationShape, _spawnCount, _bottomFormation.FormationLayerCount, spacing);

                SpawnShips(formation, pair.Key);
            }
        }
        _isSpawned = true;
    }

    private void SpawnShips((Dictionary<List<Vector3>, int>, Dictionary<List<Quaternion>, int>) formation, Transform spawnLocation)
    {
        int layer = 0;

        foreach (var layerPosition in formation.Item1)
        {
            layer = layerPosition.Value;

            foreach (var positions in layerPosition.Key)
            {
                int id = layerPosition.Value;
                Vector3 position = positions;

                if (layer == 1)
                {
                    _ship = Instantiate(_enemyShipPrefab, spawnLocation.position + position, Quaternion.identity, spawnLocation);
                }
                else if (layer == 2)
                {
                    _ship = Instantiate(_enemyShipPrefab, spawnLocation.position + position, Quaternion.identity, spawnLocation);
                }
                else if (layer == 3)
                {
                    _ship = Instantiate(_enemyShipPrefab, spawnLocation.position + position, Quaternion.identity, spawnLocation);
                }
                else if (layer == 4)
                {
                    _ship = Instantiate(_enemyShipPrefab, spawnLocation.position + position, Quaternion.identity, spawnLocation);
                }
                else if (layer == 5)
                {
                    _ship = Instantiate(_enemyShipPrefab, spawnLocation.position + position, Quaternion.identity, spawnLocation);
                }

                foreach (var layerRotation in formation.Item2)
                {
                    foreach (var rotations in layerRotation.Key)
                    {
                        int rotId = layerRotation.Value;

                        if (id == rotId)
                        {
                            Quaternion rotation = rotations;
                            _ship.transform.rotation = rotation;

                            if (spawnLocation == _leftSpawnLocation)
                            {
                                _ship.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                            }

                            if (spawnLocation == _rightSpawnLocation)
                            {
                                _ship.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 270));
                            }

                            if (spawnLocation == _topSpawnLocation)
                            {
                                _ship.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                            }

                            if (spawnLocation == _bottomSpawnLocation)
                            {
                                _ship.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                            }
                        }
                    }
                }
            }
        }
    }

    private void MoveToDestination(Transform currentLocation, Transform DestinationLocation)
    {
        currentLocation.position = Vector3.Lerp(currentLocation.position, DestinationLocation.position, lerp);

        if (currentLocation.position == DestinationLocation.position)
        {
            _isSpawned = false;
        }
    }
    
    private void PopulateSpawnPoints()
    {
        _spawnPoints = new Dictionary<Transform, Transform>();
        _spawnPoints.Add(_leftSpawnLocation, _leftDestinationLocation);
        _spawnPoints.Add(_rightSpawnLocation, _rightDestinationLocation);
        _spawnPoints.Add(_topSpawnLocation, _topDestinationLocation);
        _spawnPoints.Add(_bottomSpawnLocation, _bottomDestinationLocation);
    }

    public Transform GetRandomSpawnLocation()
    {
        int randomIndex = Random.Range(0, 4);
        return randomIndex switch
        {
            0 => _leftSpawnLocation,
            1 => _rightSpawnLocation,
            2 => _topSpawnLocation,
            3 => _bottomSpawnLocation,
            _ => _leftSpawnLocation
        };
    }
}
