using UnityEngine;

public class CuttingCounter : BaseCounter
{
    [SerializeField] private CuttingReceipeSO[] cutReceipeSOArray;
    public override void Interact(Player player)
    {
        if (!HasKitchenObject()) // There is no kitchen Object here
        {
            if (player.HasKitchenObject()) // Player is carrying something
            {
                if (HasReceipeInput(player.GetKitchenObject().GetKitchenObjectSO())) // Player Carrying something can be cut
                {
                    player.GetKitchenObject().SetKitchenObjectParent(this); 
                }
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
        if (HasKitchenObject() && HasReceipeInput(GetKitchenObject().GetKitchenObjectSO())) // There is a Kitchen Object here and can be cut
        {
            KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());
            // Cut the Object
            GetKitchenObject().DestroySelf();
            KitchenObject.SpawnKitchenObject(outputKitchenObjectSO,this);
        }
    }

    private bool HasReceipeInput(KitchenObjectSO inputKitchenObjectSO)
    {
         foreach (CuttingReceipeSO cuttingReceipeSO in cutReceipeSOArray)
        {
            if (cuttingReceipeSO.input == inputKitchenObjectSO)
            {
                return true;
            }
        }
        return false;
    }
    
    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (CuttingReceipeSO cuttingReceipeSO in cutReceipeSOArray)
        {
            if (cuttingReceipeSO.input == inputKitchenObjectSO)
            {
                return cuttingReceipeSO.output;
            }
        }
        return null;
    }
}
