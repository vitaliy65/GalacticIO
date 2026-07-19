using System.Collections.Generic;
using tiles;
using UnityEngine;

public class EmbientGenerator : Generator
{
    [SerializeField]
    public List<SerializablePair<TileBiomes, EnvironmentList>> BiomeEnvironmentLists = new List<SerializablePair<TileBiomes, EnvironmentList>>();
    public static EmbientGenerator Instance { get; private set; }

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

    public GameObject GenerateEnvironment(TileBiomes biome, TileSubBiomes subBiome, bool isOre)
    {
        var list = BiomeEnvironmentLists.Find(el => el.Key == biome && el.Value != null && el.Value.subBiome == subBiome);

        return CalculateSpawn(isOre, list.Value);
    }

    private GameObject CalculateSpawn(bool isOre, EnvironmentList list)
    {
        if (isOre)
            return TileUtils.GetResourcePrefabFromBiome(list.mainBiome);

        if (list.EnvironmentMaping == null || list.EnvironmentMaping.Count == 0)
            return null;

        float totalWeight = 0f;
        foreach (var option in list.EnvironmentMaping)
            totalWeight += option.Key;

        float roll = Random.value * totalWeight;
        float current = 0f;

        foreach (var option in list.EnvironmentMaping)
        {
            current += option.Key;
            if (roll <= current)
                if (option.Value)
                    return option.Value;
        }

        return null;
    }

    public static Quaternion ApplyRandomRotation()
    {
        float angle = Random.Range(0f, 360f);
        float snappedAngle = Mathf.Round(angle / 45f) * 45f;
        return Quaternion.Euler(0f, snappedAngle, 0f);
    }

}
