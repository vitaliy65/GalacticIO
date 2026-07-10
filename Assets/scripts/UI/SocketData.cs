using builds;
using currency;
using UnityEngine;

public class SocketData : MonoBehaviour
{
    [SerializeField]
    public BuildingData buildingData;

    public void OnClick()
    {
        bool canBuy = CurrencyManager.Instance.CanBuyByCoins(buildingData.BaseCost);

        if (canBuy && TileSelectionManager.SelectedTileGlobal && buildingData)
        {
            TileSelectionManager.SelectedTileGlobal.OnTilePlaced(buildingData);
            TileSelectionManager.Instance?.RequestMenuHide();
        }
    }
}
