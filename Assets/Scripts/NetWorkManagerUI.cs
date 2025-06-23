using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetWorkManagerUI : MonoBehaviour
{
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button startClientButton;

   

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        Hide();
    }
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        Hide();
    }
    private void Hide()
    {
        gameObject.SetActive( false );
    }
}
