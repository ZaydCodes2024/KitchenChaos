using UnityEngine;

public class ClearCounter : BaseCounter
{
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
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) // Kitchen Object is a plate
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                    }
                }
                else // Player is not carrying a plate but something else
                {
                    if (GetKitchenObject().TryGetPlate(out plateKitchenObject)) // Counter is holding a plate
                    {
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))
                        {
                            KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
                        }
                    }
                }
            }
            else // Player is carrying nothing
            {
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }
}
