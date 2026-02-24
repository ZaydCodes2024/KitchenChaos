using UnityEngine.UI;
using UnityEngine;

public class PlatesIconSingleUI : MonoBehaviour
{
    [SerializeField] private Image image;
    public void SetKitchenObjectSO(KitchenObjectSO kitchenObjectSO)
    {
        image.sprite = kitchenObjectSO.prefabSprite;
    }
}
