using UnityEngine;
using TMPro;
using System;
public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI receipesDeliveredText;

    private void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        
    }

    private void GameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsGameOver())
        {
            gameObject.SetActive(true);
            receipesDeliveredText.text = DeliveryManager.Instance.GetSuccessfullReceipeAmount().ToString();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
