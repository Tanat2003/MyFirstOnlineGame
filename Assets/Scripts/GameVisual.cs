using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameVisual : NetworkBehaviour
{
    private const float GRID_SIZE = 3.1f;

    [SerializeField] private Transform crossPrefab;
    [SerializeField] private Transform circlePrefab;
    [SerializeField] private Transform lineCompletePrefab;
    private List<GameObject> visualGameObjectList;

    private void Awake()
    {
        visualGameObjectList = new List<GameObject>();
    }

    private void Start()
    {

        GameManager.Instance.OnClickOnGridPosition += GameManager_OnClickOnGridPos;
        GameManager.Instance.OnRematach += GameManager_OnRematach;
        GameManager.Instance.OnGameWin += GameManager_OnGameWin;


    }

    private void GameManager_OnRematach(object sender, System.EventArgs e)
    {
        foreach (GameObject go in visualGameObjectList)
        {
            Destroy(go);
        }
        visualGameObjectList.Clear();
    }

    private void GameManager_OnGameWin(object sender, GameManager.OnGameWinEventArgs e)
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        float eulerZ = 0f;
        switch (e.line.orientation)
        {
            default:
            case GameManager.Orientation.Horizontal: eulerZ = 0f;break;
            case GameManager.Orientation.Vertical: eulerZ = 90f;break;
            case GameManager.Orientation.DiagonalA: eulerZ = 45f;break;
            case GameManager.Orientation.DiagonalB: eulerZ = -45f;break;

        }
        Transform lineCompleteTrasfrom =
        Instantiate(lineCompletePrefab, GetGridWorldPosition
            (e.line.centerGridPosition.x, e.line.centerGridPosition.y), Quaternion.Euler(0,0,eulerZ));

        lineCompleteTrasfrom.GetComponent<NetworkObject>().Spawn(true);

        visualGameObjectList.Add(lineCompleteTrasfrom.gameObject);
    }

    private void GameManager_OnClickOnGridPos(object sender, GameManager.OnClickOnGridPositionEventArgs e)
    {
        SpawnObjectRpc(e.x, e.y,e.playerType);

    }

    //RPC §◊Õ„ÀÈ‡¡∏Õ¥√—π„π‰ÀπÕ¬Ë“ßµÕπ·√°Client “¡“√∂‡¢È“∂÷ßGameManager_OnClickOnGridPos‰¥È·µË¡—πerror‡æ√“–NetworkObject
    //.Spawn¡—π √È“ßobj¢÷Èπ∫π‡´‘ø·≈–µÈÕß„™Èrpc√—π®“°‡§√◊ËÕßclient
    //°Ø
    //(‡¡∏Õ¥∑’Ë„™ÈrpcµÈÕß≈ß∑È“¬Rpc¥È«¬)≈–°Á §√‘ªµÏ∑’Ë¡’RpcµÈÕß ◊∫∑Õ¥®“°NetworkBehav‰¡Ëß—Èπ®–„™ÈAttributeRpc‰¡Ë‰¥È∂È“„ÀÈobjspawn∑’Ë‡¥’¬«°—πµÈÕß
    //„ ËComponentNetWorkTransform¥È«¬
    [Rpc(SendTo.Server)]
    private void SpawnObjectRpc(int x, int y,GameManager.PlayerType playerType)
    {
        Transform prefab;
        switch (playerType)
        {
            default:
            case GameManager.PlayerType.Cross:
                prefab = crossPrefab;
                break;
            case GameManager.PlayerType.Circle:
                prefab = circlePrefab;

                break;
        }

        Transform SpawnObj = Instantiate(prefab, GetGridWorldPosition(x, y), Quaternion.identity);
        SpawnObj.GetComponent<NetworkObject>().Spawn(true); //Spawn Object„π Global Space(server)
        visualGameObjectList.Add(SpawnObj.gameObject);
    }


    private Vector2 GetGridWorldPosition(int x, int y)
    {


        return new Vector2(-GRID_SIZE + x * GRID_SIZE, -GRID_SIZE + y * GRID_SIZE);
    }
}
