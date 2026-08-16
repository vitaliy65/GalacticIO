using System.Collections.Generic;
using tiles;
using UnityEngine;


[CreateAssetMenu(fileName = "EnvironmentList", menuName = "Game/forest/ForestEnvironmentList")]
public class EnvironmentList : ScriptableObject
{
    [SerializeField]
    public TileBiomes mainBiome;
    [SerializeField]
    public TileSubBiomes subBiome;
    // A float value represents the percentage change of spawning a particular GameObject
    [SerializeField]
    public List<SerializablePair<float, GameObject>> EnvironmentMaping = new List<SerializablePair<float, GameObject>>();

    [SerializeField]
    public float maxScaleCoefficient = 1f;
    [SerializeField]
    public float minScaleCoefficient = 0.9f;

    [SerializeField]
    public Vector2 maxOffsetFromCenter = new Vector2(0.5f, 0.5f);
    [SerializeField]
    public Vector2 minOffsetFromCenter = new Vector2(0.1f, 0.1f);
}
