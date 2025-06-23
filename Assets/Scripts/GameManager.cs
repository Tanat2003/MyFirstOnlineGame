using System;
using System.Collections.Generic;
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
    public event EventHandler<OnGameWinEventArgs> OnGameWin;
    public class OnGameWinEventArgs : EventArgs
    {
        public Line line;
    }
    public event EventHandler OnCurrentPlayAblePlayerTypeChange;
    public enum PlayerType
    {
        None,
        Cross,
        Circle,
    }
    public enum Orientation
    {
        Horizontal,
        Vertical,
        DiagonalA,
        DiagonalB,

    }
    public struct Line
    {
        public List<Vector2Int> gridVector2IntList;
        public Vector2Int centerGridPosition;
        public Orientation orientation;
    }
    
    private PlayerType localPlayerType;
    private List<Line> lineList;
    private NetworkVariable<PlayerType> currentPlayAblePlayerType = new NetworkVariable<PlayerType>(); //§Ë“°≈“ß∑’Ë®–„™È„πServer°—∫clientµ≈Õ¥
    private PlayerType[,] playerTypeArray;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Have more 1 GameManager");
        }
        Instance = this;
        playerTypeArray = new PlayerType[3, 3];

        lineList = new List<Line>
        {
            //Horizontal Line
            new Line{
                gridVector2IntList = new List<Vector2Int>
                {new Vector2Int(0,0),new Vector2Int(1,0),
                    new Vector2Int(2,0)},
                centerGridPosition = new Vector2Int(1,0),
               orientation = Orientation.Horizontal
            },
            new Line{
                gridVector2IntList = new List<Vector2Int>
                {new Vector2Int(0,1),new Vector2Int(1,1),
                    new Vector2Int(2, 1)},
                centerGridPosition = new Vector2Int(1,1),
                orientation = Orientation.Horizontal

            },
            new Line{
                gridVector2IntList = new List<Vector2Int>
                {new Vector2Int(0, 2),new Vector2Int(1,2),
                    new Vector2Int(2,2)},
                centerGridPosition = new Vector2Int(1,2),
                orientation = Orientation.Horizontal

            },

            //Vertical line
            new Line{
                gridVector2IntList = new List<Vector2Int>
                {new Vector2Int(0,0),new Vector2Int(0,1),
                    new Vector2Int(0,2)},
                centerGridPosition = new Vector2Int(0,1),
                orientation = Orientation.Vertical

            },
            new Line{
                gridVector2IntList = new List<Vector2Int>
                {new Vector2Int(1,0),new Vector2Int(1,1),
                    new Vector2Int(1, 2)},
                centerGridPosition = new Vector2Int(1,1),
                orientation = Orientation.Vertical

            },
            new Line{
                gridVector2IntList = new List<Vector2Int>
                {new Vector2Int(2, 0),new Vector2Int(2, 1),
                    new Vector2Int(2,2)},
                centerGridPosition = new Vector2Int(2, 1),
                orientation = Orientation.Vertical

            },
            //‡©’¬ß
            new Line{
                gridVector2IntList = new List<Vector2Int>
                {new Vector2Int(0,0),new Vector2Int(1,1),
                    new Vector2Int(2, 2)},
                centerGridPosition = new Vector2Int(1,1),
                orientation = Orientation.DiagonalA

            },
            new Line{
                gridVector2IntList = new List<Vector2Int>
                {new Vector2Int(0, 2),new Vector2Int(1, 1),
                    new Vector2Int(2,0)},
                centerGridPosition = new Vector2Int(1, 1),
                orientation = Orientation.DiagonalB

            },



        };
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
            OnCurrentPlayAblePlayerTypeChange?.Invoke(this, EventArgs.Empty);
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

        if (playerTypeArray[x, y] != PlayerType.None) //Gridπ—Èπ‚¥π„™È‰ª·≈È«
            return;

        playerTypeArray[x, y] = playerType;

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
        TestWin();


    }
    private bool TestWinnerLine(PlayerType aLine, PlayerType bLine, PlayerType cLine) =>
        aLine != PlayerType.None && aLine == bLine && bLine == cLine;


    private bool TestWinnerLineWithLineStruct(Line line)
    {
        return TestWinnerLine(
            playerTypeArray[line.gridVector2IntList[0].x, line.gridVector2IntList[0].y],
            playerTypeArray[line.gridVector2IntList[1].x, line.gridVector2IntList[1].y],
            playerTypeArray[line.gridVector2IntList[2].x, line.gridVector2IntList[2].y]
        );

    }
    private void TestWin()
    {
        foreach(Line line in lineList)
        {
            if(TestWinnerLineWithLineStruct(line))
            {
                Debug.Log("Winner");
                currentPlayAblePlayerType.Value = PlayerType.None;
                OnGameWin?.Invoke(this, new OnGameWinEventArgs
                {
                    line = line
                });
                break;
            }
        }
        
    }
    public PlayerType GetLocalPlayerType() => localPlayerType;
    public PlayerType GetCurrentPlayAblePlayerType() => currentPlayAblePlayerType.Value;
}
