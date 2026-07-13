using System;
using System.Collections.Generic;
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
        private List<SerializablePair<TileTypes, TileScriptable>> biomeResourceMapping = new List<SerializablePair<TileTypes, TileScriptable>>();

        [SerializeField]
        private List<SerializablePair<TileBiomes, ResourceData>> biomeResourceDataMapping = new List<SerializablePair<TileBiomes, ResourceData>>();

        [SerializeField]
        private List<SerializablePair<TileBiomes, Material>> biomeMaterialMapping = new List<SerializablePair<TileBiomes, Material>>();

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
                case TileBiomes.Grassland:
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

        public static Material GetTileMaterialFromBiome(TileBiomes biome)
        {
            foreach (var mapping in Instance.biomeMaterialMapping)
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
            foreach (var mapping in Instance.biomeResourceMapping)
            {
                if (mapping.Key == tileType)
                {
                    return mapping.Value;
                }
            }
            return null;
        }
    }
}