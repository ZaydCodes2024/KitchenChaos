using System;
using Unity.Netcode;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress
{
    [SerializeField] private CuttingReceipeSO[] cutReceipeSOArray;
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler OnCut;
    public static event EventHandler OnAnyCut;
    new public static void ResetStaticData()
    {
        OnAnyCut = null;
    }
    private int cuttingProgress;
    public override void Interact(Player player)
    {
        if (!HasKitchenObject()) // There is no kitchen Object here
        {
            if (player.HasKitchenObject()) // Player is carrying something
            {
                if (HasReceipeInput(player.GetKitchenObject().GetKitchenObjectSO())) // Player Carrying something can be cut
                {
                    KitchenObject kitchenObject = player.GetKitchenObject();
                    kitchenObject.SetKitchenObjectParent(this);
                    InteractLogicPlaceObjectOnCounterServerRpc();
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
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) // Kitchen Object is a plate
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                    }
                }
            }
            else // Player is carrying nothing
            {
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void InteractLogicPlaceObjectOnCounterServerRpc()
    {
        InteractLogicPlaceObjectOnCounterClientRpc();
    }

    [ClientRpc]
    private void InteractLogicPlaceObjectOnCounterClientRpc()
    {
        cuttingProgress = 0;
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = 0f
        });
    }
    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject() && HasReceipeInput(GetKitchenObject().GetKitchenObjectSO())) // There is a Kitchen Object here and can be cut
        {
            CutObjectServerRpc();
            TestCuttingProgressDoneServerRpc();
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void CutObjectServerRpc()
    {
        if (HasKitchenObject() && HasReceipeInput(GetKitchenObject().GetKitchenObjectSO())) // There is a Kitchen Object here and can be cut
        {
            CutObjectClientRpc();
        }
    }

    [ClientRpc]
    private void CutObjectClientRpc()
    {
        cuttingProgress++;

        OnCut?.Invoke(this, EventArgs.Empty);
        OnAnyCut?.Invoke(this, EventArgs.Empty);

        CuttingReceipeSO cuttingReceipeSO = GetCuttingReceipeSOInput(GetKitchenObject().GetKitchenObjectSO());

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = (float)cuttingProgress / cuttingReceipeSO.cuttingProgressMax
        });


    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void TestCuttingProgressDoneServerRpc()
    {
        if (HasKitchenObject() && HasReceipeInput(GetKitchenObject().GetKitchenObjectSO())) // There is a Kitchen Object here and can be cut
        {
            CuttingReceipeSO cuttingReceipeSO = GetCuttingReceipeSOInput(GetKitchenObject().GetKitchenObjectSO());

            if (cuttingProgress >= cuttingReceipeSO.cuttingProgressMax)
            {
                KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());
                // Cut the Object
                KitchenObject.DestroyKitchenObject(GetKitchenObject());
                KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
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