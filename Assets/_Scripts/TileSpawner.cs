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

    private List<GameObject> currentTiles;
    private List<GameObject> currentObstacles;

    private void Start()
    {
        currentTiles = new List<GameObject>();
        currentObstacles = new List<GameObject>();

        Random.InitState(System.DateTime.Now.Millisecond); // gives the date in milliseconds to determine the tile seed. Think like minecraft seeds

        for (int i = 0; i < tileStartCount; ++i)
        {
            SpawnTiles(startingTile.GetComponent<Tile>());
        }

        SpawnTiles(SelectRandomGameObjectFromList(turnTiles).GetComponent<Tile>());
    }

    private void SpawnTiles(Tile tile, bool spawnObstacle = false)
    {
        Quaternion newTileRoation =tile.gameObject.transform.rotation * Quaternion.LookRotation(currentTileDirection, Vector3.up);

        previousTile = GameObject.Instantiate(tile.gameObject, currentTileLocation, newTileRoation);
        currentTiles.Add(previousTile);

        if (spawnObstacle) SpawnObstacle();

        if(tile.type == TileType.STRAIGHT)
        {
            currentTileLocation += Vector3.Scale(previousTile.GetComponent<Renderer>().bounds.size, currentTileDirection);
        }
        
    }

    private void DeletePreviousTiles()
    {
        // eventually swap this to an object pool
        while(currentTiles.Count != 1)
        {
            GameObject tile = currentTiles[0];
            currentTiles.RemoveAt(0);
            Destroy(tile);
        }

        while (currentObstacles.Count != 0)
        {
            GameObject obstacle = currentObstacles[0];
            currentObstacles.RemoveAt(0);
            Destroy(obstacle);
        }
    }

    public void  AddNewDirection(Vector3 direction)
    {
        // eventually swap this to an object pool
        currentTileDirection = direction;
        DeletePreviousTiles();

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
