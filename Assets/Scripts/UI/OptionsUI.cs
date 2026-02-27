using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    public static OptionsUI Instance { get; private set;}
    [SerializeField] private Button soundEffectsButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button moveUpButton;
    [SerializeField] private Button moveDownButton;
    [SerializeField] private Button moveLeftButton;
    [SerializeField] private Button moveRightButton;
    [SerializeField] private Button interactButton;
    [SerializeField] private Button interactAltButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button gamepadInteractButton;
    [SerializeField] private Button gamepadInteractAltButton;
    [SerializeField] private Button gamepadPauseButton;
    [SerializeField] private TextMeshProUGUI soundEffectsButtonText;
    [SerializeField] private TextMeshProUGUI musicButtonText;
    [SerializeField] private TextMeshProUGUI moveUpText;
    [SerializeField] private TextMeshProUGUI moveDownText;
    [SerializeField] private TextMeshProUGUI moveLeftText;
    [SerializeField] private TextMeshProUGUI moveRightText;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI interactAltText;
    [SerializeField] private TextMeshProUGUI pauseText;
    [SerializeField] private TextMeshProUGUI gamepadInteractText;
    [SerializeField] private TextMeshProUGUI gamepadInteractAltText;
    [SerializeField] private TextMeshProUGUI gamepadPauseText;
    [SerializeField] private Transform pressToRebindKeyTransform;
    
    private Action OnCloseButtonAction;

    private void Awake()
    {
        Instance = this;

        soundEffectsButton.onClick.AddListener( () =>
        {
            SoundManager.Instance.ChangeVolume();
            UpdateVisual();
        } );

        musicButton.onClick.AddListener( () =>
        {
            MusicManager.Instance.ChangeVolume();
            UpdateVisual();
        } );

        closeButton.onClick.AddListener( () =>
        {
            Hide();
            OnCloseButtonAction();
        } );

        moveUpButton.onClick.AddListener( () =>
        {
            RebindBinding(GameInput.Binding.MoveUp);
        } );

        moveDownButton.onClick.AddListener( () =>
        {
            RebindBinding(GameInput.Binding.MoveDown);
        } );

        moveLeftButton.onClick.AddListener( () =>
        {
            RebindBinding(GameInput.Binding.MoveLeft);
        } );

        moveRightButton.onClick.AddListener( () =>
        {
            RebindBinding(GameInput.Binding.MoveRight);
        } );

        interactButton.onClick.AddListener( () =>
        {
            RebindBinding(GameInput.Binding.Interact);
        } );

        interactAltButton.onClick.AddListener( () =>
        {
            RebindBinding(GameInput.Binding.InteractAlt);
        } );

        pauseButton.onClick.AddListener( () =>
        {
            RebindBinding(GameInput.Binding.Pause);
        } );

        gamepadInteractButton.onClick.AddListener( () =>
        {
            RebindBinding(GameInput.Binding.GamepadInteract);
        } );

        gamepadInteractAltButton.onClick.AddListener( () =>
        {
            RebindBinding(GameInput.Binding.GamepadInteractAlt);
        } );

        gamepadPauseButton.onClick.AddListener( () =>
        {
            RebindBinding(GameInput.Binding.GamepadPause);
        } );
    }

    private void Start()
    {
        GameManager.Instance.OnGameUnpaused += GameManager_OnGameUnpaused;
        UpdateVisual();
        HidePressToRebindKey();
        Hide();
    }

    private void GameManager_OnGameUnpaused(object sender, EventArgs e)
    {
        Hide();
    }

    private void UpdateVisual()
    {
        soundEffectsButtonText.text = "Sound Effects: " + Mathf.Round(SoundManager.Instance.GetVolume() * 10f);
        musicButtonText.text = "Music: " + Mathf.Round(MusicManager.Instance.GetVolume() * 10f);

        moveUpText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveUp);
        moveDownText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveDown);
        moveLeftText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveLeft);
        moveRightText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveRight);
        interactText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
        interactAltText.text = GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlt);
        pauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);
        gamepadInteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.GamepadInteract);
        gamepadInteractAltText.text = GameInput.Instance.GetBindingText(GameInput.Binding.GamepadInteractAlt);
        gamepadPauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.GamepadPause);

    }

    public void Show(Action OnCloseButtonAction)
    {
        this.OnCloseButtonAction = OnCloseButtonAction;
        gameObject.SetActive(true);
        soundEffectsButton.Select();
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void ShowPressToRebindKey()
    {
        pressToRebindKeyTransform.gameObject.SetActive(true);
    }
    private void HidePressToRebindKey()
    {
        pressToRebindKeyTransform.gameObject.SetActive(false);
    }

    private void RebindBinding(GameInput.Binding binding)
    {
        ShowPressToRebindKey();
        GameInput.Instance.RebindBinding(binding, () => {
            HidePressToRebindKey();
            UpdateVisual();
        });
    }
}
