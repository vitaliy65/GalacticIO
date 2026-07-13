using System.Collections.Generic;
using builds;
using UnityEngine;

namespace tiles
{
    public abstract class TileBehavior : MonoBehaviour
    {
        private static readonly HashSet<TileBehavior> allTiles = new HashSet<TileBehavior>();

        public static IReadOnlyCollection<TileBehavior> AllTiles => allTiles;

        protected virtual void OnEnable()
        {
            allTiles.Add(this);
        }

        protected virtual void OnDisable()
        {
            allTiles.Remove(this);
        }

        [SerializeField]
        protected TileScriptable tileData;
        [SerializeField]
        protected TileStates tileState = TileStates.Empty;
        [SerializeField]
        protected Building tileBuilding = null;
        [SerializeField]
        protected ResourceData tileResourceData;
        [SerializeField]
        protected Material SelectedMaterial;
        [SerializeField]
        protected Material HoveredMaterial;
        [SerializeField]
        protected GameObject OutlinedPart;
        [SerializeField]
        protected GameObject BuildAnchorPoint;

        public bool isSelected { get; set; }
        public bool isHovered { get; set; }


        public float Height { get; private set; }
        public float Heat { get; private set; }

        public void SetGeneratedMapData(float height, float heat)
        {
            Height = height;
            Heat = heat;
        }

        // Expose read-only accessors so other systems can decide UI/logic without
        // changing the protected serialized fields directly.
        public TileScriptable TileData => tileData;
        public ResourceData TileResourceData => tileResourceData;
        public TileStates TileState
        {
            get => tileState;
            set => tileState = value;
        }
        public Building TileBuilding
        {
            get => tileBuilding;
            set => tileBuilding = value;
        }


        public abstract void OnTileSelected();
        public abstract void OnTileUnselected();
        public abstract void OnTileHovered();
        public abstract void OnTileUnhovered();
        public abstract void OnTilePlaced(Building building);
        public abstract bool OnTileRemoved();
    }
}