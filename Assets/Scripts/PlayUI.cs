using System;
using UnityEngine;

public class PlayUI : MonoBehaviour
{
    [SerializeField] private GameObject crossArrowGameObject;
    [SerializeField] private GameObject circleArrowGameObject;
    [SerializeField] private GameObject crossYouTextGameObject;
    [SerializeField] private GameObject circleYouTextGameObject;


    private void Awake()
    {
        crossArrowGameObject.SetActive(false);
        circleArrowGameObject.SetActive(false);
        crossYouTextGameObject.SetActive(false);
        circleYouTextGameObject.SetActive(false);

    }
    private void Start()
    {
        GameManager.Instance.OnGameStarted += Gamemanager_OnGameStarted;
        GameManager.Instance.OnCurrentPlayAblePlayerTypeChange += Gamemanager_OnCurrentPlayAblePlayerTypeChange;
    }

    private void Gamemanager_OnCurrentPlayAblePlayerTypeChange(object sender, EventArgs e)
    {   
        UpdateCurrentArrow();
    }

    private void Gamemanager_OnGameStarted(object sender, System.EventArgs e)
    {
        if(GameManager.Instance.GetLocalPlayerType() == GameManager.PlayerType.Cross)
        {
            crossYouTextGameObject.SetActive(true);
        }
        else
        {
            circleYouTextGameObject.SetActive(true) ;
        }
        UpdateCurrentArrow();
    }

    private void UpdateCurrentArrow()
    {
       if(GameManager.Instance.GetCurrentPlayAblePlayerType() == GameManager.PlayerType.Cross)
        {
            crossArrowGameObject.SetActive(true);
            circleArrowGameObject.SetActive(false) ;

        }
        else
        {
            crossArrowGameObject.SetActive(false);
            circleArrowGameObject.SetActive(true);
        }
    }
}
