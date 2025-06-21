
using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    public event EventHandler<OnClickOnGridPositionEventArgs> OnClickOnGridPosition;
    public class OnClickOnGridPositionEventArgs : EventArgs
    {
        public int x;
        public int y;
        public PlayerType playerType;
    }

    public event EventHandler OnGameStarted;
    public event EventHandler OnCurrentPlayAblePlayerTypeChange;
    public enum PlayerType
    {
        None,
        Cross,
        Circle,
    }
    private PlayerType localPlayerType;
    private NetworkVariable<PlayerType> currentPlayAblePlayerType= new NetworkVariable<PlayerType>(); //¤èÒ¡ÅÒ§·Õè¨Ðãªéã¹Server¡ÑºclientµÅÍ´

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Have more 1 GameManager");
        }
        Instance = this;
    }
    public override void OnNetworkSpawn()
    {
        Debug.Log("OnNetWorkSpawn " + NetworkManager.Singleton.LocalClientId);
        if (NetworkManager.Singleton.LocalClientId == 0)
        {
            localPlayerType = PlayerType.Cross;
        }
        else
        {
            localPlayerType = PlayerType.Circle;
        }
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += NetWorkManager_OnClientConnectedCallback;
        }
        currentPlayAblePlayerType.OnValueChanged += (PlayerType oldPlayerType, PlayerType newPlayerType) =>
        {
            OnCurrentPlayAblePlayerTypeChange?.Invoke(this,EventArgs.Empty);
        };
    }

    private void NetWorkManager_OnClientConnectedCallback(ulong obj)
    {
        if (NetworkManager.Singleton.ConnectedClientsList.Count == 2)
        {
            //StartGame
            currentPlayAblePlayerType.Value = PlayerType.Cross;
            TriggerOnGameStartedRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameStartedRpc()
    {
        OnGameStarted?.Invoke(this, EventArgs.Empty);
    }

    [Rpc(SendTo.Server)]
    public void ClickOnGridPositionRpc(int x, int y, PlayerType playerType)
    {
        Debug.Log("Click" + x + ", " + y);
        if (playerType != currentPlayAblePlayerType.Value)
            return;


        OnClickOnGridPosition?.Invoke(this,
            new OnClickOnGridPositionEventArgs
            {
                x = x,
                y = y,
                playerType = playerType,

            });

        switch (currentPlayAblePlayerType.Value)
        {
            default:
            case PlayerType.Cross:
                currentPlayAblePlayerType.Value = PlayerType.Circle;
                break;
            case PlayerType.Circle:
                currentPlayAblePlayerType.Value = PlayerType.Cross;
                break;


        }

        
    }
    
    public PlayerType GetLocalPlayerType() => localPlayerType;
    public PlayerType GetCurrentPlayAblePlayerType() => currentPlayAblePlayerType.Value;
}
