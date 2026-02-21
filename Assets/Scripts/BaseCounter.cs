using UnityEngine;

public class BaseCounter : MonoBehaviour, IKitchenObjectParent
{
    private KitchenObject kitchenObject;
    [SerializeField] private Transform counterTopPoint;
    public  virtual void Interact(Player player)
    {
        
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
