using builds;
using currency;
using UnityEngine;

public class SocketData : MonoBehaviour
{
    [SerializeField]
    public BuildingData buildingData;

    public void OnClick()
    {
        if (!buildingData || !TileSelectionManager.SelectedTileGlobal)
        {
            return;
        }

        if (!CurrencyManager.Instance.TrySpendCoins(buildingData.BaseCost))
        {
            return;
        }

        TileSelectionManager.SelectedTileGlobal.OnTilePlaced(buildingData);
        TileSelectionManager.Instance?.RequestMenuHide();
        UIManager.Instance.RefreshCurrencyText();
    }
}
