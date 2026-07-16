using System.Collections.Generic;
using tiles;
using UnityEngine;


[CreateAssetMenu(fileName = "EnvironmentList", menuName = "Game/forest/ForestEnvironmentList")]
public class EnvironmentList : ScriptableObject
{
    // A float value represents the percentage change of spawning a particular GameObject
    [SerializeField]
    public List<SerializablePair<float, GameObject>> EnvironmentMaping = new List<SerializablePair<float, GameObject>>();

    [SerializeField]
    public GameObject OrePrefab;
}
