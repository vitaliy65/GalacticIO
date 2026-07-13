
using tiles;

public class DeleteSocket : Socket
{

    public override void OnClick()
    {
        TileBehavior selectedTile = TileSelectionManager.SelectedTileGlobal;

        if (!selectedTile)
        {
            return;
        }

        selectedTile.OnTileRemoved();
        HandleSuccess();
        TileSelectionManager.Instance.ClearSelection();
    }
}
