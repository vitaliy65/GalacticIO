using UnityEngine;

public abstract class Socket : MonoBehaviour
{
    public abstract void OnClick();

    protected void HandleSuccess()
    {
        TileSelectionManager.Instance?.RequestMenuHide();
        UIManager.Instance.RefreshCurrencyText();
    }
}
