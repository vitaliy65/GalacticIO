using UnityEngine;
using builds;

namespace tiles
{
    /// <summary>
    /// Spawns a full hex-grid world. For every cell in a rectangular
    /// mapWidthInTiles x mapHeightInTiles range, it:
    ///   1. Samples height/heat at that cell's world position (HexBiomeGenerator).
    ///   2. Picks a TileBiomes from height/heat (Whittaker-style lookup).
    ///   3. Rolls a separate, independent noise mask to decide whether this
    ///      specific cell is part of a small ore cluster.
    ///   4. Looks up the right TileScriptable / ResourceData / Material for the
    ///      result (TileUtils) and spawns+initializes a tile there.
    ///
    /// Ore clusters are intentionally a SEPARATE noise map from biome height/heat,
    /// sampled at a much smaller feature size and thresholded high, so they come
    /// out as small, sparse blobs scattered across a biome instead of the biome
    /// being made of ore wholesale. A biome's material/base type comes purely
    /// from height+heat; whether any individual tile within it also happens to
    /// be an ore-bearing tile is decided independently.
    ///
    /// Uses the scene's existing hex Grid component (GetCellCenterWorld) to
    /// convert (column, row) cell coordinates into world positions, so spawned
    /// tiles line up with the exact spacing Unity already uses for hand-painted
    /// hex tiles - no hex math is reimplemented here.
    /// </summary>
    public class WorldGenerator : Generator
    {
        [Header("Grid")]
        [Tooltip("The scene's hex Grid component (Hexagon cell layout). Used only to convert (col,row) into a correctly-spaced world position.")]
        [SerializeField] private Grid hexGrid;
        [Tooltip("The tile prefab to spawn at every cell. All biome variation comes from data/material assigned after spawning, not from separate prefabs per biome.")]
        [SerializeField] private GameObject tilePrefab;
        [Tooltip("Optional parent for spawned tiles, just for scene hierarchy tidiness.")]
        [SerializeField] private Transform tilesParent;
        [SerializeField] private int mapWidthInTiles = 20;
        [SerializeField] private int mapHeightInTiles = 20;

        [Header("Height & heat source")]
        [Tooltip("Reuses the same noise settings/curves as HexBiomeGenerator, so a world generated here matches what Generate() would produce for the same positions.")]
        [SerializeField] private BiomeGenerator biomeGenerator;

        [Header("Ore clusters")]
        [Tooltip("Size of ore clusters, in world units. Small values = small, tight clusters. Keep this well below heightFeatureSize on the biome generator, otherwise clusters will roughly follow biome shapes instead of looking scattered.")]
        [SerializeField] private float oreClusterFeatureSize = 4f;
        [Tooltip("How high the cluster noise has to be for a tile to count as ore, 0-1. Higher = rarer, smaller clusters. Start around 0.75-0.85.")]
        [SerializeField, Range(0f, 1f)] private float oreClusterThreshold = 0.8f;
        [SerializeField] private Vector2 oreClusterNoiseOrigin = new Vector2(5000f, 5000f);

        public static WorldGenerator Instance { get; private set; }

        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        [ContextMenu("Generate World")]
        public void GenerateWorld()
        {
            if (!ValidateReferences())
            {
                return;
            }

            ClearTilesParentChildren();

            for (int row = 0; row < mapHeightInTiles; row++)
            {
                for (int col = 0; col < mapWidthInTiles; col++)
                {
                    SpawnTile(col, row);
                }
            }
        }

        private void SpawnTile(int col, int row)
        {
            Vector3 worldPosition = hexGrid.GetCellCenterWorld(new Vector3Int(col, row, 0));

            float Height = biomeGenerator.SampleHeight(worldPosition);
            float heat = biomeGenerator.SampleHeat(worldPosition);
            float moisture = biomeGenerator.SampleMoisture(worldPosition);
            TileBiomes biome = BiomeGenerator.PickBiome(Height, heat, moisture);

            if (biome == TileBiomes.Ocean)
            {
                return;
            }

            TileSubBiomes subBiome = biomeGenerator.PickSubBiome(biome, Height, moisture, worldPosition);

            // Ore doesn't make sense under Ocean tiles - everything else is
            // eligible for a small chance of an ore cluster.
            bool isOreCluster = biome != TileBiomes.Ocean && IsOreCluster(worldPosition);
            TileTypes tileType = isOreCluster ? TileTypes.Ore : TileTypes.Ground;

            TileScriptable tileData = TileUtils.GetTileDataFromType(tileType);
            ResourceData resourceData = isOreCluster ? TileUtils.GetResourceDataFromBiome(biome) : null;
            // Material biomeMaterial = TileUtils.GetTileMaterialFromBiome(biome);

            if (!tileData)
            {
                Debug.LogWarning($"WorldGenerator: no TileScriptable mapped for {tileType} in TileUtils - cell ({col},{row}) will be spawned without tile data.", this);
            }

            GameObject spawnedTile = Instantiate(tilePrefab, worldPosition, Quaternion.identity, tilesParent);
            spawnedTile.name = $"Tile_{col}_{row}_{biome}_{subBiome}";


            Tile tile = spawnedTile.GetComponent<Tile>();
            tile.InitializeFromWorldGenerator(tileData, resourceData, Height, heat, biome, subBiome);
            tile.AddEnvironment(EmbientGenerator.Instance.GenerateEnvironment(biome, subBiome, isOreCluster));
        }

        private bool IsOreCluster(Vector3 worldPosition)
        {
            // A small, low-octave noise map sampled at a much smaller feature
            // size than height/heat, so it varies quickly across the grid -
            // that's what makes the clusters small relative to a biome instead
            // of following the same broad shapes as the terrain itself.
            float clusterNoise = NoiseUtils.FractalNoise(
                worldPosition.x, worldPosition.z,
                octaves: 2, persistence: 0.5f, lacunarity: 2f,
                scale: oreClusterFeatureSize, offset: oreClusterNoiseOrigin);

            return clusterNoise > oreClusterThreshold;
        }

        /// <summary>
        /// Removes all children from the optional tilesParent transform.
        /// Safe to call in editor or play mode; uses DestroyImmediate in the
        /// editor when not playing so the hierarchy updates immediately.
        /// </summary>
        public void ClearTilesParentChildren()
        {
            foreach (Transform child in tilesParent.transform)
            {
                Destroy(child.gameObject);
            }
        }

        private bool ValidateReferences()
        {
            if (!hexGrid)
            {
                Debug.LogError("WorldGenerator: hexGrid is not assigned.", this);
                return false;
            }

            if (!tilePrefab)
            {
                Debug.LogError("WorldGenerator: tilePrefab is not assigned.", this);
                return false;
            }

            if (!biomeGenerator)
            {
                Debug.LogError("WorldGenerator: biomeGenerator is not assigned.", this);
                return false;
            }

            if (!TileUtils.Instance)
            {
                Debug.LogError("WorldGenerator: no TileUtils found in the scene (its mappings are needed to resolve tile data/resources/materials).", this);
                return false;
            }

            return true;
        }
    }
}
