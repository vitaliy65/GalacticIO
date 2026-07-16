using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using builds;
using UnityEngine;

namespace tiles
{
    [Serializable]
    public struct SerializablePair<TKey, TValue>
    {
        public TKey Key;
        public TValue Value;
    }

    public class TileUtils : MonoBehaviour
    {
        [SerializeField]
        private List<SerializablePair<TileTypes, TileScriptable>> TileTypeMapping = new List<SerializablePair<TileTypes, TileScriptable>>();

        [SerializeField]
        private List<SerializablePair<TileBiomes, ResourceData>> biomeResourceDataMapping = new List<SerializablePair<TileBiomes, ResourceData>>();

        [SerializeField]
        private HeatmapList heatmapList;

        public static TileUtils Instance { get; private set; }

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



        public static TileResources GetResourceFromBiome(TileBiomes biome)
        {
            switch (biome)
            {
                case TileBiomes.Desert:
                    return TileResources.Iron;
                case TileBiomes.Forest:
                    return TileResources.Copper;
                case TileBiomes.Plains:
                    return TileResources.Gold;
                case TileBiomes.Mountain:
                    return TileResources.Silver;
                case TileBiomes.Ocean:
                    return TileResources.None;
                case TileBiomes.Tundra:
                    return TileResources.Diamond;
                default:
                    return TileResources.None;
            }
        }

        public static ResourceData GetResourceDataFromBiome(TileBiomes biome)
        {
            foreach (var mapping in Instance.biomeResourceDataMapping)
            {
                if (mapping.Key == biome)
                {
                    return mapping.Value;
                }
            }
            return null;
        }



        // public static Material GetTileMaterialFromBiome(TileBiomes biome)
        // {
        //     foreach (var mapping in Instance.biomeMaterialMapping)
        //     {
        //         if (mapping.Key == biome)
        //         {
        //             return mapping.Value;
        //         }
        //     }
        //     return null;
        // }


        public static TileScriptable GetTileDataFromType(TileTypes tileType)
        {
            foreach (var mapping in Instance.TileTypeMapping)
            {
                if (mapping.Key == tileType)
                {
                    return mapping.Value;
                }
            }
            return null;
        }

        /// <summary>
        /// Looks up a color for a given heat value (0-1) from heatColorGradient -
        /// a thin horizontal gradient texture where X position = heat and the
        /// pixel color at that X is the color to show. GetPixelBilinear smoothly
        /// blends between neighboring pixels, and Y is fixed at the middle row so
        /// the texture can be any height (even 1px tall works).
        /// Returns white (no tint) if TileUtils or the texture isn't set up yet,
        /// so a missing reference doesn't throw and break tile initialization.
        /// </summary>
        public static Color GetColor(float height, TileBiomes biome)
        {
            // Ограничиваем на всякий случай в диапазоне [0, 1]
            float pixelCoordinate = Mathf.Clamp01(height);

            Texture2D heatColorGradient = null;

            foreach (var option in Instance.heatmapList.HeatmapMaping)
            {
                if (option.Key == biome)
                    heatColorGradient = option.Value;
            }

            return heatColorGradient.GetPixelBilinear(pixelCoordinate, pixelCoordinate);
        }
    }
}