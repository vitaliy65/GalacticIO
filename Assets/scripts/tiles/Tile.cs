using UnityEngine;
using tiles;
using builds;

public class Tile : TileBehavior
{

    public override void OnTileSelected()
    {
        isSelected = true;
    }
    public override void OnTileUnselected()
    {
        isSelected = false;
    }
    public override void OnTileHovered()
    {
        if (!isSelected)
        {
            isHovered = true;
        }
    }
    public override void OnTileUnhovered()
    {
        if (!isSelected)
        {
            isHovered = false;
        }
    }
    public override void OnTilePlaced(Building building)
    {
        GameObject currentOccupiedObject = SpawnAnchorPoint.transform.GetChild(0).gameObject;

        // delete the current object if it exists
        if (currentOccupiedObject != null)
            Destroy(currentOccupiedObject);

        if (TileState == TileStates.Empty)
        {
            GameObject currentBuilding = Instantiate(building.BuildingData.BuildingPrefab, SpawnAnchorPoint.transform, false);
            currentBuilding.GetComponent<Building>().IsPlacedOnResource = tileResourceData;
            TileBuilding = currentBuilding.GetComponent<Building>();
        }

        TileState = TileStates.Occupied;
    }
    public override bool OnTileRemoved()
    {
        if (SpawnAnchorPoint.transform.childCount > 0)
        {
            TileBuilding = null;
            Transform child = SpawnAnchorPoint.transform.GetChild(0);

            // Unregister before destroying so BuildingManager never holds on to
            // a reference to a Building whose GameObject no longer exists.
            Building buildingComponent = child.GetComponent<Building>();
            if (buildingComponent != null && BuildingManager.Instance != null)
            {
                BuildingManager.Instance.RemoveBuilding(buildingComponent);
            }

            Destroy(child.gameObject);

            TileState = TileStates.Empty;
            return true;
        }

        return false;
    }

    public void OnMouseEnter()
    {
        if (!isHovered)
        {
            OnTileHovered();
        }
    }

    public void OnMouseExit()
    {
        if (isHovered)
        {
            OnTileUnhovered();
        }
    }
}
