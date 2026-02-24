using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class DeliveryManagerSingleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI receipeNameText;
    [SerializeField] private Transform iconContainer;
    [SerializeField] private Transform iconTemplate;
    
    private void Awake()
    {
        iconTemplate.gameObject.SetActive(false);
    }

    public void SetReceipeSO(ReceipeSO receipeSO)
    {
        receipeNameText.text = receipeSO.receipeName;

        foreach (Transform child in iconContainer)
        {
            if (child == iconTemplate) continue;
                Destroy(child.gameObject);            
        }

        foreach (KitchenObjectSO kitchenObjectSO in receipeSO.kitchenObjectSOList)
        {
            Transform iconTransform = Instantiate(iconTemplate, iconContainer);
            iconTransform.gameObject.SetActive(true);
            iconTransform.GetComponent<Image>().sprite = kitchenObjectSO.prefabSprite;
        }
    }
}
