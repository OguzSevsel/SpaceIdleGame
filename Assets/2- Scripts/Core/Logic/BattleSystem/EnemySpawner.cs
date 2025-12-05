using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private Dictionary<Transform, Transform> _spawnPoints;
    [SerializeField] private Transform _leftSpawnLocation;
    [SerializeField] private Transform _rightSpawnLocation;
    [SerializeField] private Transform _topSpawnLocation;
    [SerializeField] private Transform _bottomSpawnLocation;
    [SerializeField] private Transform _leftDestinationLocation;
    [SerializeField] private Transform _rightDestinationLocation;
    [SerializeField] private Transform _topDestinationLocation;
    [SerializeField] private Transform _bottomDestinationLocation;
    [SerializeField] private Transform _playerFleetLocation;
    [SerializeField] private Formation _leftFormation;
    [SerializeField] private Formation _rightFormation;
    [SerializeField] private Formation _topFormation;
    [SerializeField] private Formation _bottomFormation;
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
            List<Vector3> spots = FormationGenerator.Generate(_leftFormation.FormationShape, _spawnCount, _leftFormation.FormationLayerCount, 0.5f);

            if (pair.Key == _rightSpawnLocation)
            {
                spots = FormationGenerator.Generate(_rightFormation.FormationShape, _spawnCount, _rightFormation.FormationLayerCount, 0.5f);
            }

            if (pair.Key == _topSpawnLocation)
            {
                spots = FormationGenerator.Generate(_topFormation.FormationShape, _spawnCount, _topFormation.FormationLayerCount, 0.5f);
            }

            if (pair.Key == _bottomSpawnLocation)
            {
                spots = FormationGenerator.Generate(_bottomFormation.FormationShape, _spawnCount, _bottomFormation.FormationLayerCount, 0.5f);
            }
            Debug.Log($"Spawn Count: {_spawnCount}, Spots Count: {spots.Count}");

            foreach (var pos in spots)
            {
                _ship = Instantiate(_enemyShipPrefab, pair.Key.transform.position + pos, Quaternion.identity, pair.Key.transform);

                if (pair.Key == _leftSpawnLocation)
                {
                    _ship.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                }

                if (pair.Key == _rightSpawnLocation)
                {
                    _ship.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 270));
                }

                if (pair.Key == _topSpawnLocation)
                {
                    _ship.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                }

                if (pair.Key == _bottomSpawnLocation)
                {
                    _ship.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                }
            }
        }
        _isSpawned = true;
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
