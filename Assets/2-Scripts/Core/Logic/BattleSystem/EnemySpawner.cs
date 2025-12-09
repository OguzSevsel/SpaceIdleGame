using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class EnemySpawner : MonoBehaviour
{
    private Dictionary<Transform, SpawnSide> _spawnPoints;
    public enum SpawnSide { Left, Right, Top, Bottom }

    [SerializeField] private Transform _leftSpawnLocation;
    [SerializeField] private Formation _leftFormation;

    [SerializeField] private Transform _rightSpawnLocation;
    [SerializeField] private Formation _rightFormation;

    [SerializeField] private Transform _topSpawnLocation;
    [SerializeField] private Formation _topFormation;

    [SerializeField] private Transform _bottomSpawnLocation;
    [SerializeField] private Formation _bottomFormation;

    [SerializeField] private Transform _playerFleetLocation;
    [SerializeField] private GameObject _enemyShipPrefab;
    [SerializeField] private int _spawnCount = 1;

    public float spawnOffset = 100f;
    public float innerOffset = 2f;

    private GameObject _ship;
    private bool _isSpawned = false;
    private float lerp = 0.01f;

    //This logic can be moved to a Coroutine for better performance
    private void Update()
    {
        if (_isSpawned)
        {
            _isSpawned = false;  // run only once

            foreach (KeyValuePair<Transform, SpawnSide> pair in _spawnPoints)
            {
                StartCoroutine(MoveObject(pair.Key.gameObject, pair.Value));
            }
        }
    }

    private void Start()
    {

    }

    //private void FormationCreation()
    //{
    //    PopulateSpawnPoints();
    //    foreach (KeyValuePair<Transform, SpawnSide> pair in _spawnPoints)
    //    {
    //        float spacing = 2f;

    //        (Dictionary<List<Vector3>, int>, Dictionary<List<Quaternion>, int>) formation = FormationGenerator.Generate(_leftFormation.Shape, _spawnCount, _leftFormation.LayerCount, spacing);
    //        SpawnShips(formation, pair.Key);

    //        if (pair.Value == SpawnSide.Right)
    //        {
    //            formation = FormationGenerator.Generate(_rightFormation.Shape, _spawnCount, _rightFormation.LayerCount, spacing);
    //            SpawnShips(formation, pair.Key);
    //        }

    //        if (pair.Value == SpawnSide.Top)
    //        {
    //            formation = FormationGenerator.Generate(_topFormation.Shape, _spawnCount, _topFormation.LayerCount, spacing);
    //            SpawnShips(formation, pair.Key);
    //        }

    //        if (pair.Value == SpawnSide.Bottom)
    //        {
    //            formation = FormationGenerator.Generate(_bottomFormation.Shape, _spawnCount, _bottomFormation.LayerCount, spacing);

    //            SpawnShips(formation, pair.Key);
    //        }
    //    }
    //    _isSpawned = true;
    //}

    //private void SpawnShips((Dictionary<List<Vector3>, int>, Dictionary<List<Quaternion>, int>) formation, Transform spawnLocation)
    //{
    //    int layer = 0;

    //    foreach (var layerPosition in formation.Item1)
    //    {
    //        layer = layerPosition.Value;

    //        foreach (var positions in layerPosition.Key)
    //        {
    //            int id = layerPosition.Value;
    //            Vector3 position = positions;

    //            if (layer == 1)
    //            {
    //                _ship = Instantiate(_enemyShipPrefab, spawnLocation.position + position, Quaternion.identity, spawnLocation);
    //            }
    //            else if (layer == 2)
    //            {
    //                _ship = Instantiate(_enemyShipPrefab, spawnLocation.position + position, Quaternion.identity, spawnLocation);
    //            }
    //            else if (layer == 3)
    //            {
    //                _ship = Instantiate(_enemyShipPrefab, spawnLocation.position + position, Quaternion.identity, spawnLocation);
    //            }
    //            else if (layer == 4)
    //            {
    //                _ship = Instantiate(_enemyShipPrefab, spawnLocation.position + position, Quaternion.identity, spawnLocation);
    //            }
    //            else if (layer == 5)
    //            {
    //                _ship = Instantiate(_enemyShipPrefab, spawnLocation.position + position, Quaternion.identity, spawnLocation);
    //            }

    //            foreach (var layerRotation in formation.Item2)
    //            {
    //                foreach (var rotations in layerRotation.Key)
    //                {
    //                    int rotId = layerRotation.Value;

    //                    if (id == rotId)
    //                    {
    //                        Quaternion rotation = rotations;
    //                        _ship.transform.rotation = rotation;

    //                        if (spawnLocation == _leftSpawnLocation)
    //                        {
    //                            _ship.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
    //                        }

    //                        if (spawnLocation == _rightSpawnLocation)
    //                        {
    //                            _ship.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 270));
    //                        }

    //                        if (spawnLocation == _topSpawnLocation)
    //                        {
    //                            _ship.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
    //                        }

    //                        if (spawnLocation == _bottomSpawnLocation)
    //                        {
    //                            _ship.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}


    #region Initial Movement Logic

    // ---------------------- Replace MoveObject ----------------------
    IEnumerator MoveObject(GameObject obj, SpawnSide side)
    {
        Camera cam = Camera.main;
        if (cam == null) yield break;
        if (obj == null) yield break;

        Transform t = obj.transform;

        // Compute world start/end on the same Z as the object (important)
        Vector3 start = GetSpawnPoint(cam, side, spawnOffset, t);
        Vector3 end = GetTargetPoint(cam, side, innerOffset, t);

        // Put container exactly at start (children move with it)
        t.position = start;

        // Movement parameters
        float duration = 2f;      // total travel time (tweak)
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float n = Mathf.Clamp01(elapsed / duration);

            // Ease-out curve (fast -> slow). This one is gentle:
            float smooth = 1f - Mathf.Pow(1f - n, 2f);   // quadratic ease-out

            // Straight-line interpolation with ease-out
            t.position = Vector3.Lerp(start, end, smooth);

            yield return null;
        }

        // Snap exactly to the end (prevents tiny floating-point offset)
        t.position = end;
    }

    // ---------------------- Replace GetSpawnPoint ----------------------
    private Vector3 GetSpawnPoint(Camera cam, SpawnSide side, float offset = 5f, Transform obj = null)
    {
        // z distance from camera to the object's plane (if obj null use 0)
        float zDistance = Mathf.Abs(cam.transform.position.z - (obj != null ? obj.position.z : 0f));

        switch (side)
        {
            case SpawnSide.Left:
                return cam.ScreenToWorldPoint(new Vector3(0f - offset, cam.pixelHeight / 2f, zDistance));

            case SpawnSide.Right:
                return cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth + offset, cam.pixelHeight / 2f, zDistance));

            case SpawnSide.Top:
                return cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth / 2f, cam.pixelHeight + offset, zDistance));

            case SpawnSide.Bottom:
                return cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth / 2f, 0f - offset, zDistance));
        }

        return Vector3.zero;
    }

    // ---------------------- Replace GetTargetPoint ----------------------
    private Vector3 GetTargetPoint(Camera cam, SpawnSide side, float innerOffset = 5f, Transform obj = null)
    {
        float zDistance = Mathf.Abs(cam.transform.position.z - (obj != null ? obj.position.z : 0f));

        switch (side)
        {
            case SpawnSide.Left:
                return cam.ScreenToWorldPoint(new Vector3(innerOffset, cam.pixelHeight / 2f, zDistance));

            case SpawnSide.Right:
                return cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth - innerOffset, cam.pixelHeight / 2f, zDistance));

            case SpawnSide.Top:
                return cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth / 2f, cam.pixelHeight - innerOffset, zDistance));

            case SpawnSide.Bottom:
                return cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth / 2f, innerOffset, zDistance));
        }

        return Vector3.zero;
    }

    #endregion


    private void PopulateSpawnPoints()
    {
        _spawnPoints = new Dictionary<Transform, SpawnSide>();
        _spawnPoints.Add(_leftSpawnLocation, SpawnSide.Left);
        _spawnPoints.Add(_rightSpawnLocation, SpawnSide.Right);
        _spawnPoints.Add(_topSpawnLocation, SpawnSide.Top);
        _spawnPoints.Add(_bottomSpawnLocation, SpawnSide.Bottom);
    }


    private void OnDrawGizmos()
    {
        if (Camera.main == null) return;

        Camera cam = Camera.main;

        // Sides to visualize
        SpawnSide[] sides = { SpawnSide.Left, SpawnSide.Right, SpawnSide.Top, SpawnSide.Bottom };

        Gizmos.color = Color.yellow;

        foreach (var side in sides)
        {
            Vector3 spawn = GetSpawnPoint(cam, side, spawnOffset);
            Vector3 target = GetTargetPoint(cam, side, innerOffset);

            // Spawn sphere
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(spawn, 0.3f);

            // Target sphere
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(target, 0.3f);

            // Line between them
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(spawn, target);
        }
    }
}
