using System;
using System.Collections;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
    [SerializeField] private BaseCounter baseCounter;
    [SerializeField] private GameObject[] visualCounterObject;
    private void Start()
    {
        if (Player.localInstance != null)
        {
            Player.localInstance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
        }
        else
        {
            Player.OnAnyPlayerSpawned += Player_OnAnyPlayerSpawned;
        }
        
    }

    private void Player_OnAnyPlayerSpawned(object sender, EventArgs e)
    {
        if (Player.localInstance != null)
        {
            Player.localInstance.OnSelectedCounterChanged -= Player_OnSelectedCounterChanged;
            Player.localInstance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
        }
    }

    private void Player_OnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArgs e)
    {
        if (e.selectedCounter == baseCounter)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        foreach (GameObject visualGameObject in visualCounterObject)
            visualGameObject.SetActive(true);
    }
    private void Hide()
    {
        foreach (GameObject visualGameObject in visualCounterObject)
            visualGameObject.SetActive(false);
    }
}
