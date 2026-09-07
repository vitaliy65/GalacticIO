using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace tiles
{
    public static class TileRegistry
    {
        private static readonly HashSet<TileBehavior> allTiles = new HashSet<TileBehavior>();

        public static IReadOnlyCollection<TileBehavior> AllTiles => allTiles;

        public static void Register(TileBehavior tile)
        {
            if (tile)
            {
                allTiles.Add(tile);
            }
        }

        public static void Unregister(TileBehavior tile)
        {
            if (tile)
            {
                allTiles.Remove(tile);
            }
        }

        public static TileBehavior PickRandomTile()
        {
            if (allTiles.Count == 0)
            {
                return null;
            }

            int randomIndex = Random.Range(0, allTiles.Count);
            return allTiles.ElementAt(randomIndex);
        }

        public static TileBehavior PickRandomTileWithoutHills()
        {
            List<TileBehavior> availableTiles = allTiles
                .Where(tile => tile.SubBiome != TileSubBiomes.Hills)
                .ToList();

            if (availableTiles.Count == 0)
            {
                return null;
            }

            int randomIndex = Random.Range(0, availableTiles.Count);
            return availableTiles[randomIndex];
        }
    }
}