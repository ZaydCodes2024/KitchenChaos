using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using Unity.Netcode;
public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI receipesDeliveredText;
    [SerializeField] private Button playAgainButton;

    private void Awake()
    {
        playAgainButton.onClick.AddListener( () =>
        {
           NetworkManager.Singleton.Shutdown();
           Loader.Load(Loader.Scene.MainMenuScene); 
        } 
        );
    }
    private void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        Hide();
    }

    private void GameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsGameOver())
        {
            Show();
            receipesDeliveredText.text = DeliveryManager.Instance.GetSuccessfullReceipeAmount().ToString();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
        playAgainButton.Select();
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
