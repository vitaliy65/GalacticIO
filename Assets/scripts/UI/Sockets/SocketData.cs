using builds;
using currency;
using tiles;
using UnityEngine;

public class SocketData : Socket
{
    [SerializeField]
    public Building buildingData;

    public override void OnClick()
    {
        TileBehavior selectedTile = TileSelectionManager.SelectedTileGlobal;

        if (!buildingData || !selectedTile)
        {
            return;
        }

        if (!CurrencyManager.Instance.TrySpendCoins(buildingData.BuildingData.BaseCost))
        {
            return;
        }

        selectedTile.OnTilePlaced(buildingData);
        HandleSuccess();
        TileSelectionManager.Instance.ClearSelection();
    }
}
