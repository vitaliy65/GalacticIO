using UnityEngine;

namespace tiles
{
    [CreateAssetMenu(fileName = "TileScriptable", menuName = "Scriptable Objects/TileScriptable")]
    public class TileScriptable : ScriptableObject
    {
        [SerializeField] private TileTypes tileType = TileTypes.Ground;
        [SerializeField] private bool _canBePlacedOn = false;
        [SerializeField] private int _health = 250;

        public TileTypes Type { get => tileType; set => tileType = value; }
        public int health { get => _health; set => _health = value; }
        public bool canBePlacedOn { get => _canBePlacedOn; set => _canBePlacedOn = value; }
    }
}