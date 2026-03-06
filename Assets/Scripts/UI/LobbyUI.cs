using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button quickJoinLobbyButton;
    [SerializeField] private Button JoinCodeButton;
    [SerializeField] private LobbyCreateUI lobbyCreateUI;
    [SerializeField] private TMP_InputField joinCodeInputField;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener( ()=>
        {
           Loader.Load(Loader.Scene.MainMenuScene); 
        });
        createLobbyButton.onClick.AddListener( ()=>
        {
           lobbyCreateUI.Show();
        });
        quickJoinLobbyButton.onClick.AddListener( ()=>
        {
           KitchenGameLobby.Instance.QuickJoin(); 
        });
        JoinCodeButton.onClick.AddListener( ()=>
        {
           KitchenGameLobby.Instance.JoinWithCode(joinCodeInputField.text); 
        });
    }
}
