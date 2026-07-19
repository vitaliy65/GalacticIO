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
        private List<SerializablePair<TileBiomes, GameObject>> biomeResourcePrefabMapping = new List<SerializablePair<TileBiomes, GameObject>>();

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

        public static GameObject GetResourcePrefabFromBiome(TileBiomes biome)
        {
            foreach (var mapping in Instance.biomeResourcePrefabMapping)
            {
                if (mapping.Key == biome)
                {
                    return mapping.Value;
                }
            }
            return null;
        }


        public static Color GetColor(float height, TileBiomes biome)
        {
            // Ограничиваем на всякий случай в диапазоне [0, 1]
            float pixelCoordinate = Mathf.Clamp01(height);

            if (pixelCoordinate >= 0.99)
            {
                pixelCoordinate -= 0.01f;
            }

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