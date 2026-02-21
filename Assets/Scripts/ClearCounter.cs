using UnityEngine;

public class ClearCounter : MonoBehaviour, IKitchenObjectParent
{
    private KitchenObject kitchenObject;
    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    [SerializeField] private Transform counterTopPoint;
    public void Interact(Player player)
    {
        if (kitchenObject == null)
        {
            Transform kitchenObjectTransfom = Instantiate(kitchenObjectSO.prefab, counterTopPoint);
            kitchenObjectTransfom.GetComponent<KitchenObject>().SetKitchenObjectParent(this);
        }
        else
        {   // Give object to player
            kitchenObject.SetKitchenObjectParent(player);
        }
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return counterTopPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }
}
