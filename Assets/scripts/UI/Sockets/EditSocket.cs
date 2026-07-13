using builds;
using tiles;

public class EditSocket : Socket
{
    public override void OnClick()
    {
        TileBehavior selectedTile = TileSelectionManager.SelectedTileGlobal;

        if (!selectedTile)
        {
            return;
        }

        if (BuildingManager.Instance?.UpgradeBuilding(selectedTile.TileBuilding) == true)
        {
            HandleSuccess();
        }
    }
}
