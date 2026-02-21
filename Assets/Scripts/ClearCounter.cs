using UnityEngine;

public class ClearCounter : MonoBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    [SerializeField] private Transform counterTopPoint;
    public void Interact()
    {
        Transform kitchenObjectTransfom = Instantiate(kitchenObjectSO.prefab, counterTopPoint);
        kitchenObjectTransfom.localPosition = Vector3.zero;

        Debug.Log(kitchenObjectTransfom.GetComponent<KitchenObject>().GetKitchenObjectSO().name);
    }
}
