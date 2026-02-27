using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryUI : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Color successColor;
    [SerializeField] private Color failedColor;
    [SerializeField] private Sprite successSprite;
    [SerializeField] private Sprite failedSprite;
    [SerializeField] private Animator animator;
    private const string POPUP = "Popup";
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        DeliveryManager.Instance.OnReceipeSuccess += DeliveryManager_OnReceipeSuccess;
        DeliveryManager.Instance.OnReceipeFailed += DeliveryManager_OnReceipeFailed;
        gameObject.SetActive(false);
    }

    private void DeliveryManager_OnReceipeFailed(object sender, EventArgs e)
    {
        gameObject.SetActive(true);
        backgroundImage.color = failedColor;
        iconImage.sprite = failedSprite;
        messageText.text = "DELIVERY\nFAILED";
        animator.SetTrigger(POPUP);
    }

    private void DeliveryManager_OnReceipeSuccess(object sender, EventArgs e)
    {
        gameObject.SetActive(true);
        backgroundImage.color = successColor;
        iconImage.sprite = successSprite;
        messageText.text = "DELIVERY\nSUCCESS";
        animator.SetTrigger(POPUP);
    }
}
