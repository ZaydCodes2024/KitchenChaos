using UnityEngine;

public class CuttingCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO cutKitchenObjectSO;
    public override void Interact(Player player)
    {
        if (!HasKitchenObject()) // There is no kitchen Object here
        {
            if (player.HasKitchenObject()) // Player is carrying something
            {
                player.GetKitchenObject().SetKitchenObjectParent(this); 
            }
            else 
            {
                // Player is carrying nothing
            }
        }
        else // There is a Kitchen Object here
        {
            if (player.HasKitchenObject()) // Player is Carrying something
            {
                // Do nothing
            }
            else // Player is carrying nothing
            {
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject()) // There is a Kitchen Object here
        {
            // Cut the Object
            GetKitchenObject().DestroySelf();
            KitchenObject.SpawnKitchenObject(cutKitchenObjectSO,this);
        }
    }
}
