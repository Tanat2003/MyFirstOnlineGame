using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resultTxt;
    [SerializeField] private Color winColor;
    [SerializeField] private Color loseColor;
    [SerializeField] private Button rematchButton;



    private void Awake()
    {
        rematchButton.onClick.AddListener(() =>
        {
            GameManager.Instance.RematchRpc();
        });
    }

    private void Start()
    {
        GameManager.Instance.OnGameWin += GameManager_OnGameWin;
        GameManager.Instance.OnRematach += GameManager_OnRematach;
        GameManager.Instance.OnGameTie += GameManager_OnGameTie;
        Hide();
    }

    private void GameManager_OnGameTie(object sender, System.EventArgs e)
    {
        resultTxt.text = "Tie!";
        resultTxt.color = loseColor;
        Show();
    }

    private void GameManager_OnRematach(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void GameManager_OnGameWin(object sender, GameManager.OnGameWinEventArgs e)
    {
        if(e.winPlayerType == GameManager.Instance.GetLocalPlayerType())
        {
            resultTxt.text = "You Win";
            resultTxt.color = winColor;
            Show();
            
        }
        else
        {
            resultTxt.text = "You Lose";
            resultTxt.color = loseColor;
            Show();
            
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
