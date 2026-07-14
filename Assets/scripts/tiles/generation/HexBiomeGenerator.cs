using UnityEngine;

namespace tiles
{
    /// <summary>
    /// Computes height (elevation) and heat (temperature) for hex tiles.
    ///
    /// Two ways to use it:
    /// - Generate() - post-processes tiles that already exist in the scene
    ///   (via TileBehavior.AllTiles), e.g. hand-painted ones.
    /// - SampleHeight()/SampleHeat() - compute a value for an arbitrary world
    ///   position, for code that needs the answer BEFORE a tile exists there
    ///   (e.g. WorldGenerator, deciding what to spawn at each grid cell).
    ///
    /// IMPORTANT - why this samples noise using world position and not a
    /// row/column index:
    /// Unity's hex Grid already spaces tiles correctly for a hex layout (every
    /// other row is offset horizontally so hexagons tile without gaps). If you
    /// instead fed raw row/column indices into the noise function, every row
    /// would be treated as if it weren't offset, which shows up as a visible
    /// diagonal "stretching"/shearing in the generated pattern. Sampling at the
    /// real world position sidesteps this completely, since Unity has already
    /// solved the spacing problem for you.
    ///
    /// This project's grid lies on the XY plane (confirmed from the scene: hex
    /// neighbor offsets like (2.56, 1.48, 0) - Z is constant). If you ever move
    /// to an XZ-plane (3D, Y-up) layout instead, swap Y for Z below.
    /// </summary>
    public class HexBiomeGenerator : MonoBehaviour
    {
        [Header("Height noise (elevation)")]
        [Tooltip("How many noise layers are stacked together. More layers add finer detail on top of the big shapes, at a higher cost per tile. 3-5 is typical.")]
        [SerializeField] private int heightDetailLayerCount = 4;
        [Tooltip("How much each extra detail layer contributes compared to the layer before it (0-1). Higher = rougher, more detailed terrain.")]
        [SerializeField] private float heightDetailStrength = 0.5f;
        [Tooltip("How much smaller/denser each extra detail layer is compared to the layer before it. Higher = finer, busier detail.")]
        [SerializeField] private float heightDetailZoomMultiplier = 2f;
        [Tooltip("Size of the large-scale height features, in world units. Bigger = broader, smoother continents/hills; smaller = busier, noisier terrain.")]
        [SerializeField] private float heightFeatureSize = 10f;
        [Tooltip("Shifts which part of the noise pattern gets sampled. Change this to get a different-looking map from the same settings; keep it fixed to regenerate the same map.")]
        [SerializeField] private Vector2 heightNoiseOrigin;

        [Header("Heat noise (local variation on top of latitude)")]
        [SerializeField] private int heatDetailLayerCount = 3;
        [SerializeField] private float heatDetailStrength = 0.5f;
        [SerializeField] private float heatDetailZoomMultiplier = 2f;
        [SerializeField] private float heatFeatureSize = 14f;
        [SerializeField] private Vector2 heatNoiseOrigin = new Vector2(1000f, 1000f);

        [Header("Climate shaping")]
        [Tooltip("How much elevation cools a tile down. 0 = elevation has no effect on heat; higher = mountains are noticeably colder.")]
        [SerializeField] private float elevationCoolingEffect = 0.35f;
        [Tooltip("How much weight latitude has vs. local noise when computing heat. 0 = pure noise (no climate bands), 1 = pure latitude (perfectly straight bands).")]
        [SerializeField, Range(0f, 1f)] private float latitudeInfluence = 0.7f;
        [SerializeField, Range(-5f, 1f)] private float seaLevel = 0f;
        [SerializeField, Range(1f, 5f)] private float mountainLevel = 3f;

        public float SeaLevel => seaLevel;
        public float MountainLevel => mountainLevel;

        public static HexBiomeGenerator Instance { get; private set; }

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

        [ContextMenu("Generate Height & Heat For Existing Tiles")]
        public void Generate()
        {
            foreach (var tile in TileBehavior.AllTiles)
            {
                Vector3 worldPosition = tile.transform.position;

                float Height = SampleHeight(worldPosition);
                float heat = SampleHeat(worldPosition, Height);

                tile.SetGeneratedMapData(Height, heat);
            }
        }

        /// <summary>
        /// Elevation at a given world position, roughly in [0, 1]. Exposed (not
        /// private) so code like WorldGenerator can sample it for a grid cell
        /// before any tile exists there yet.
        /// </summary>
        public float SampleHeight(Vector3 worldPosition)
        {
            return NoiseUtils.FractalNoise(
                worldPosition.x, worldPosition.z,
                heightDetailLayerCount, heightDetailStrength, heightDetailZoomMultiplier, heightFeatureSize,
                heightNoiseOrigin, seaLevel, mountainLevel);

        }

        /// <summary>
        /// Heat/temperature at a given world position, roughly in [0, 1]. Needs
        /// the height at that position too, since higher elevation cools a tile
        /// down (see elevationCoolingEffect).
        /// </summary>
        public float SampleHeat(Vector3 worldPosition, float height)
        {
            // ИСПРАВЛЕНО: Заменено worldPosition.z на worldPosition.y для XY-плоскости
            float heatNoise = NoiseUtils.FractalNoise(
                worldPosition.x, worldPosition.y,
                heatDetailLayerCount, heatDetailStrength, heatDetailZoomMultiplier, heatFeatureSize,
                heatNoiseOrigin);

            float heat = Mathf.Clamp01(latitudeInfluence + heatNoise * (1f - latitudeInfluence));

            // Чем выше тайл (ближе к горным вершинам), тем сильнее падает температура
            heat -= height * elevationCoolingEffect;

            return Mathf.Clamp01(heat);
        }

        /// <summary>
        /// Классическая схема Уиттекера (Whittaker). 
        /// Теперь принимает И высоту, И теплоту для точного определения биома.
        /// </summary>
        public static TileBiomes PickBiome(float height, float heat)
        {
            // 1. Сначала проверяем жесткие физические границы высоты
            if (height <= 0)
            {
                return TileBiomes.Ocean;
            }
            if (height >= 1)
            {
                return TileBiomes.Mountain;
            }

            // 2. Для промежуточных высот (суши) распределяем биомы строго по температуре:
            // от самого холодного к самому горячему
            if (heat <= 0.5f)
            {
                return TileBiomes.Tundra; // Холодно
            }
            if (heat > 0.5f && heat <= 0.65f)
            {
                return TileBiomes.Forest; // Прохладно
            }
            if (heat > 0.65f && heat <= 0.8f)
            {
                return TileBiomes.Grassland; // Умеренно
            }

            return TileBiomes.Desert; // Жарко
        }
    }
}
