using tiles;
using UnityEngine;

public class GenerateCastle : MonoBehaviour
{
    public GameObject castlePrefab;
    public Vector3 castlePosition = new Vector3(0, 0, 0);

    private void OnEnable()
    {
        WorldGenerator.OnWorldGenerationEnd += Generate;
    }

    private void OnDisable()
    {
        WorldGenerator.OnWorldGenerationEnd -= Generate;
    }

    public void Generate()
    {
        Tile tile = (Tile)TileRegistry.PickRandomTileWithoutHills();
        if (tile == null)
            return;

        GameObject castle = Instantiate(castlePrefab, tile.SpawnAnchorPoint.transform.position, castlePrefab.transform.rotation);
        castle.transform.SetParent(tile.transform, false);
        castle.transform.position = tile.SpawnAnchorPoint.transform.position;
        castlePosition = castle.transform.position;
    }
}
