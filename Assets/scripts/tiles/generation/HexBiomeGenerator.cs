using UnityEngine;

namespace tiles
{
    /// <summary>
    /// Computes a height value and a heat value for every hex tile already placed
    /// in the scene (via TileBehavior.AllTiles) and stores them on each tile.
    ///
    /// IMPORTANT - why this samples noise using transform.position and not a
    /// row/column index:
    /// Unity's hex Grid already spaces tiles correctly for a hex layout (every
    /// other row is offset horizontally so hexagons tile without gaps). If you
    /// instead fed raw row/column indices into the noise function, every row
    /// would be treated as if it weren't offset, which shows up as a visible
    /// diagonal "stretching"/shearing in the generated pattern. Sampling at the
    /// tile's actual world position sidesteps this completely, since Unity has
    /// already solved the spacing problem for you when it placed the tiles.
    ///
    /// This project's grid lies on the XY plane (confirmed from the scene: hex
    /// neighbor offsets like (2.56, 1.48, 0) - Z is constant). If you ever move
    /// to an XZ-plane (3D, Y-up) layout instead, swap pos.y below for
    /// tile.transform.position.z.
    /// </summary>
    public class HexBiomeGenerator : MonoBehaviour
    {
        [Header("Height noise (elevation)")]
        [SerializeField] private int heightOctaves = 4;
        [SerializeField] private float heightPersistence = 0.5f;
        [SerializeField] private float heightLacunarity = 2f;
        [SerializeField] private float heightScale = 10f;
        [SerializeField] private Vector2 heightOffset;

        [Header("Heat noise (local variation on top of latitude)")]
        [SerializeField] private int heatOctaves = 3;
        [SerializeField] private float heatPersistence = 0.5f;
        [SerializeField] private float heatLacunarity = 2f;
        [SerializeField] private float heatScale = 14f;
        [SerializeField] private Vector2 heatOffset = new Vector2(1000f, 1000f);

        [Header("Climate shaping")]
        [Tooltip("How much elevation cools a tile down. 0 = elevation has no effect on heat.")]
        [SerializeField] private float heightCoolingStrength = 0.35f;
        [Tooltip("How much weight latitude has vs. local noise when computing heat (0-1).")]
        [SerializeField, Range(0f, 1f)] private float latitudeWeight = 0.7f;
        [Tooltip("World-space Y of the map's 'equator' - the warmest row.")]
        [SerializeField] private float equatorY = 0f;
        [Tooltip("Distance in world units from the equator to the coldest edge of the map.")]
        [SerializeField] private float halfMapSpanY = 30f;

        [ContextMenu("Generate Height & Heat")]
        public void Generate()
        {
            foreach (var tile in TileBehavior.AllTiles)
            {
                Vector2 pos = new Vector2(tile.transform.position.x, tile.transform.position.y);

                float height = NoiseUtils.FractalNoise(
                    pos.x, pos.y,
                    heightOctaves, heightPersistence, heightLacunarity, heightScale,
                    heightOffset);

                float heat = ComputeHeat(pos, height);

                tile.SetGeneratedMapData(height, heat);
            }
        }

        private float ComputeHeat(Vector2 pos, float height)
        {
            // Latitude band: 1 at the equator, fading toward 0 at the map's edges.
            // This is what produces large-scale climate bands instead of heat
            // being just noise with no overall structure.
            float distanceFromEquator = Mathf.Abs(pos.y - equatorY);
            float latitude = 1f - Mathf.Clamp01(distanceFromEquator / Mathf.Max(halfMapSpanY, 0.0001f));

            // A second, independent noise map for local variation - without this,
            // every tile at the same latitude would have identical heat and the
            // climate bands would look like perfectly straight stripes.
            float heatNoise = NoiseUtils.FractalNoise(
                pos.x, pos.y,
                heatOctaves, heatPersistence, heatLacunarity, heatScale,
                heatOffset);

            float heat = Mathf.Clamp01(latitude * latitudeWeight + heatNoise * (1f - latitudeWeight));

            // Higher elevation = colder, same as real mountains, even near the equator.
            heat -= height * heightCoolingStrength;

            return Mathf.Clamp01(heat);
        }

        /// <summary>
        /// Example Whittaker-style height/heat -> biome lookup (see the redblobgames
        /// article above). These thresholds are placeholders - tune them for your
        /// game. TileTypes currently only has Ground/Forest/Water/Ore/Build, so this
        /// is a starting point; add more TileScriptable assets (desert, tundra, etc.)
        /// and extend this method as you flesh biomes out further.
        /// </summary>
        public static TileTypes PickBiome(float height, float heat)
        {
            if (height < 0.35f)
            {
                return TileTypes.Water;
            }

            if (height > 0.8f)
            {
                return TileTypes.Ore; // e.g. mountains are where ore veins surface
            }

            if (heat > 0.55f)
            {
                return TileTypes.Forest;
            }

            return TileTypes.Ground;
        }
    }
}
