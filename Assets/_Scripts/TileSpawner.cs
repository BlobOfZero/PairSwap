using System.Collections.Generic;
using TempleRun;
using UnityEngine;



public class TileSpawner : MonoBehaviour
{
    [SerializeField] private int tileStartCount; // how many straight tiles will spawn at the start of the game
    [SerializeField] private int minimumStraightTiles;
    [SerializeField] private int maximumStraightTiles;
    [SerializeField] private GameObject startingTile;
    [SerializeField] private List<GameObject> turnTiles;
    [SerializeField] private List<GameObject> obstacles;
    [SerializeField] private float obstacleSpawnChance;

    private Vector3 currentTileLocation = Vector3.zero;
    private Vector3 currentTileDirection = Vector3.forward;
    private GameObject previousTile;

    private List<GameObject> currentTiles = new List<GameObject>();
    private List<GameObject> currentObstacles = new List<GameObject>();
    private Queue<GameObject> tilePool = new Queue<GameObject>();
    private Queue<GameObject> obstaclePool = new Queue<GameObject>();

    private void Start()
    {
        for (int i = 0; i < tileStartCount; ++i)
        {
            GameObject tile = InstantiateTile(startingTile);
            tilePool.Enqueue(tile);
        }

        SpawnTile(startingTile);
    }

    private GameObject InstantiateTile(GameObject prefab)
    {
        GameObject tile = Instantiate(prefab, transform);
        tile.SetActive(false);
        return tile;
    }

    private void SpawnTile(GameObject prefab, bool spawnObstacle = false)
    {
        GameObject tile = GetTileFromPool(prefab);
        tile.transform.position = CalculateNextTilePosition();
        tile.SetActive(true);
        currentTiles.Add(tile);

        if (spawnObstacle)
        {
            SpawnObstacle(tile.transform.position);
        }
    }

    private GameObject GetTileFromPool(GameObject prefab)
    {
        if (tilePool.Count > 0)
        {
            GameObject tile = tilePool.Dequeue();
            tile.SetActive(true);
            return tile;
        }
        else
        {
            return InstantiateTile(prefab);
        }
    }

    private void ReturnTileToPool(GameObject tile)
    {
        tile.SetActive(false);
        tilePool.Enqueue(tile);
    }

    private void SpawnObstacle(Vector3 position)
    {
        if (Random.value <= obstacleSpawnChance)
        {
            GameObject obstacle = GetObstacleFromPool();
            obstacle.transform.position = position;
            obstacle.SetActive(true);
            currentObstacles.Add(obstacle);
        }
    }

    private GameObject GetObstacleFromPool()
    {
        if (obstaclePool.Count > 0)
        {
            GameObject obstacle = obstaclePool.Dequeue();
            obstacle.SetActive(true);
            return obstacle;
        }
        else
        {
            int randomIndex = Random.Range(0, obstacles.Count);
            GameObject obstaclePrefab = obstacles[randomIndex];
            return InstantiateTile(obstaclePrefab);
        }
    }

    private void ReturnObstacleToPool(GameObject obstacle)
    {
        obstacle.SetActive(false);
        obstaclePool.Enqueue(obstacle);
    }

    private Vector3 CalculateNextTilePosition()
    {
        return Vector3.zero;
    }

    public void  AddNewDirection(Vector3 direction)
    {
        // eventually swap this to an object pool
        currentTileDirection = direction;
        ReturnTileToPool();

        Vector3 tilePlacementScale;
        if(previousTile.GetComponent<Tile>().type == TileType.SIDEWAYS)
        {
            tilePlacementScale = Vector3.Scale(previousTile.GetComponent<Renderer>().bounds.size / 2 + (Vector3.one *
                startingTile.GetComponent<BoxCollider>().size.z / 2), currentTileDirection);
        }
        else
        {
            tilePlacementScale = Vector3.Scale((previousTile.GetComponent<Renderer>().bounds.size - (Vector3.one * 2)) + (Vector3.one *
                startingTile.GetComponent<BoxCollider>().size.z / 2), currentTileDirection);
        }

        currentTileLocation += tilePlacementScale;

        int currentPathLength = Random.Range(minimumStraightTiles, maximumStraightTiles);
        for (int i = 0; i < currentPathLength; ++i)
        {
            SpawnTiles(startingTile.GetComponent<Tile>(), (i == 0) ? false : true);
        }

        SpawnTiles(SelectRandomGameObjectFromList(turnTiles).GetComponent<Tile>(), false);
    }

    private void SpawnObstacle()
    {
        if (Random.value > obstacleSpawnChance) return;

        GameObject obstaclePrefab = SelectRandomGameObjectFromList(obstacles);
        Quaternion newObjectRotation = obstaclePrefab.gameObject.transform.rotation * Quaternion.LookRotation(currentTileDirection, Vector3.up);

        GameObject obstastacle = Instantiate(obstaclePrefab, currentTileLocation, newObjectRotation);
        currentObstacles.Add(obstastacle);
    }

    private GameObject SelectRandomGameObjectFromList(List<GameObject> list)
    {
        if(list.Count == 0) return null;
        return list[Random.Range(0, list.Count)];
    }
}
