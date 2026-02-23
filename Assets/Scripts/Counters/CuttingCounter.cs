using System;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress
{
    [SerializeField] private CuttingReceipeSO[] cutReceipeSOArray;
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged; 
    public event EventHandler OnCut;
    private int cuttingProgress;
    public override void Interact(Player player)
    {
        if (!HasKitchenObject()) // There is no kitchen Object here
        {
            if (player.HasKitchenObject()) // Player is carrying something
            {
                if (HasReceipeInput(player.GetKitchenObject().GetKitchenObjectSO())) // Player Carrying something can be cut
                {
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    cuttingProgress = 0;
                    CuttingReceipeSO cuttingReceipeSO = GetCuttingReceipeSOInput(GetKitchenObject().GetKitchenObjectSO());
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = (float)cuttingProgress/cuttingReceipeSO.cuttingProgressMax
                    }); 
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
            cuttingProgress++;

            OnCut?.Invoke(this, EventArgs.Empty);

            CuttingReceipeSO cuttingReceipeSO = GetCuttingReceipeSOInput(GetKitchenObject().GetKitchenObjectSO());

            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = (float)cuttingProgress/cuttingReceipeSO.cuttingProgressMax
            }); 

            if (cuttingProgress >= cuttingReceipeSO.cuttingProgressMax)
            {
                KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());
                // Cut the Object
                GetKitchenObject().DestroySelf();
                KitchenObject.SpawnKitchenObject(outputKitchenObjectSO,this);
            }
            
        }
    }

    private bool HasReceipeInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingReceipeSO cuttingReceipeSO = GetCuttingReceipeSOInput(inputKitchenObjectSO);
        return cuttingReceipeSO != null;
    }
    
    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingReceipeSO cuttingReceipeSO = GetCuttingReceipeSOInput(inputKitchenObjectSO);

        if (cuttingReceipeSO != null)
        {
            return cuttingReceipeSO.output;
        }
        else
        {
            return null;
        }
    }

    private CuttingReceipeSO GetCuttingReceipeSOInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (CuttingReceipeSO cuttingReceipeSO in cutReceipeSOArray)
        {
            if (cuttingReceipeSO.input == inputKitchenObjectSO)
            {
                return cuttingReceipeSO;
            }
        }
        return null;
    }
}