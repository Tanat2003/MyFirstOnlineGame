
using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public event EventHandler<OnClickOnGridPositionEventArgs> OnClickOnGridPosition;
    public class OnClickOnGridPositionEventArgs : EventArgs
    {
        public int x;
        public int y;
    }


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Have more 1 GameManager");
        }
        Instance = this;
    }

    public void ClickOnGridPosition(int x, int y)
    {
        Debug.Log("Click" + x + ", " + y);
        OnClickOnGridPosition?.Invoke(this,
            new OnClickOnGridPositionEventArgs
            {
                x = x,
                y = y   

            });
    }
}
