using System;
using Unity.Netcode;
using UnityEngine;

public class GameVisual : MonoBehaviour
{
    private const float GRID_SIZE = 3.1f;

    [SerializeField] private Transform crossPrefab;
    [SerializeField] private Transform circlePrefab;


    
    private void Start()
    {
       
        GameManager.Instance.OnClickOnGridPosition += GameManager_OnClickOnGridPos;
          
    }

  
    private void GameManager_OnClickOnGridPos(object sender,GameManager.OnClickOnGridPositionEventArgs e)
    {
       Transform SpawnObj = Instantiate(crossPrefab);
        SpawnObj.GetComponent<NetworkObject>().Spawn(true); //Spawn Objectã¹ Global Space(server)
        SpawnObj.position =GetGridWorldPosition(e.x, e.y);

    }



    private Vector2 GetGridWorldPosition(int x, int y)
    {


        return new Vector2(-GRID_SIZE + x* GRID_SIZE, -GRID_SIZE+y *GRID_SIZE);
    }
}
