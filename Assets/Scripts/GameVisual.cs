using Unity.Netcode;
using UnityEngine;

public class GameVisual : NetworkBehaviour
{
    private const float GRID_SIZE = 3.1f;

    [SerializeField] private Transform crossPrefab;
    [SerializeField] private Transform circlePrefab;



    private void Start()
    {

        GameManager.Instance.OnClickOnGridPosition += GameManager_OnClickOnGridPos;

    }


    private void GameManager_OnClickOnGridPos(object sender, GameManager.OnClickOnGridPositionEventArgs e)
    {
        SpawnObjectRpc(e.x, e.y,e.playerType);

    }

    //RPC คือให้เมธอดรันในไหนอย่างตอนแรกClientสามารถเข้าถึงGameManager_OnClickOnGridPosได้แต่มันerrorเพราะNetworkObject
    //.Spawnมันสร้างobjขึ้นบนเซิฟและต้องใช้rpcรันจากเครื่องclient
    //กฏ
    //(เมธอดที่ใช้rpcต้องลงท้ายRpcด้วย)ละก็สคริปต์ที่มีRpcต้องสืบทอดจากNetworkBehavไม่งั้นจะใช้AttributeRpcไม่ได้ถ้าให้objspawnที่เดียวกันต้อง
    //ใส่ComponentNetWorkTransformด้วย
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
        SpawnObj.GetComponent<NetworkObject>().Spawn(true); //Spawn Objectใน Global Space(server)

    }


    private Vector2 GetGridWorldPosition(int x, int y)
    {


        return new Vector2(-GRID_SIZE + x * GRID_SIZE, -GRID_SIZE + y * GRID_SIZE);
    }
}
