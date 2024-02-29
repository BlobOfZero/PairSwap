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
            SpawnTiles(startingTile.GetComponent<Tile>(), false);
        }

        SpawnTiles(SelectRandomGameObjectFromList(turnTiles).GetComponent<Tile>());
    }

    private void SpawnTiles(Tile tile, bool spawnObstacle = false)
    {
        Quaternion newTileRoation =tile.gameObject.transform.rotation * Quaternion.LookRotation(currentTileDirection, Vector3.up);

        previousTile = GameObject.Instantiate(tile.gameObject, currentTileLocation, newTileRoation);
        currentTiles.Add(previousTile);
        currentTileLocation += Vector3.Scale(previousTile.GetComponent<Renderer>().bounds.size, currentTileDirection);
    }

    private void DeletePreviousTiles()
    {

    }

    public void  AddNewDirection(Vector3 direction)
    {
        currentTileDirection = direction;
        DeletePreviousTiles();
    }

    private GameObject SelectRandomGameObjectFromList(List<GameObject> list)
    {
        if(list.Count == 0) return null;
        return list[Random.Range(0, list.Count)];
    }
}
