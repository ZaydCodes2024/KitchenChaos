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
    [SerializeField] private TMP_InputField playerNameInputField;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener( ()=>
        {
           KitchenGameLobby.Instance.LeaveLobby();
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
    private void Start()
    {
        playerNameInputField.text = KitchenGameMultiplayer.Instance.GetPlayerName();
        playerNameInputField.onValueChanged.AddListener((string newText) =>
        {
            KitchenGameMultiplayer.Instance.SetPlayerName(newText);
        });
    }
}
