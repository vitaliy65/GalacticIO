using System.Collections.Generic;
using tiles;
using UnityEngine;

[CreateAssetMenu(fileName = "HeatmapList", menuName = "Game/heatmap/HeatmapList")]
public class HeatmapList : ScriptableObject
{
    [SerializeField]
    public List<SerializablePair<TileBiomes, Texture2D>> HeatmapMaping = new List<SerializablePair<TileBiomes, Texture2D>>();
}
