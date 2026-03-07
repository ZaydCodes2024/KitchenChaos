using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button singlePlayerPlayButton;
    [SerializeField] private Button multiPlayerPlayButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        singlePlayerPlayButton.onClick.AddListener( () =>
        {
            KitchenGameMultiplayer.playMultiplayer = false;
            Loader.Load(Loader.Scene.LobbyScene);
        } );

        multiPlayerPlayButton.onClick.AddListener(() =>
        {
            KitchenGameMultiplayer.playMultiplayer = true;
            Loader.Load(Loader.Scene.LobbyScene);
        });

        quitButton.onClick.AddListener( () =>
        {
            Application.Quit();
        } );

        Time.timeScale = 1f;
    }

}
