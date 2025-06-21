using UnityEngine;

public class GridPosition : MonoBehaviour
{

    [SerializeField] private int x;
    [SerializeField] private int y;
    private void OnMouseDown()
    {
        GameManager.Instance.ClickOnGridPositionRpc(x, y,GameManager.Instance.GetLocalPlayerType());    
    }
}
