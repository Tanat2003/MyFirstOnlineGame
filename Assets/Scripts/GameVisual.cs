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

    //RPC ���������ʹ�ѹ��˹���ҧ�͹�áClient����ö��Ҷ֧GameManager_OnClickOnGridPos�����ѹerror����NetworkObject
    //.Spawn�ѹ���ҧobj��鹺��Կ��е�ͧ��rpc�ѹ�ҡ����ͧclient
    //��
    //(���ʹ�����rpc��ͧŧ����Rpc����)�С�ʤ�Ի������Rpc��ͧ�׺�ʹ�ҡNetworkBehav����鹨���AttributeRpc����������objspawn������ǡѹ��ͧ
    //���ComponentNetWorkTransform����
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
        SpawnObj.GetComponent<NetworkObject>().Spawn(true); //Spawn Object� Global Space(server)

    }


    private Vector2 GetGridWorldPosition(int x, int y)
    {


        return new Vector2(-GRID_SIZE + x * GRID_SIZE, -GRID_SIZE + y * GRID_SIZE);
    }
}
