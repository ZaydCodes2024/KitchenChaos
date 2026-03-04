using System;
using Unity.Netcode;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress
{
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }
    public enum State
    {
        Idle, Frying, Fried, Burned
    }

    private NetworkVariable<State> currentState = new(State.Idle);
    [SerializeField] private FryingReceipeSO[] fryingReceipeSOArray;
    [SerializeField] private BurningReceipeSO[] burningReceipeSOArray;
    private NetworkVariable<float> fryTimer = new(0f);
    private NetworkVariable<float> burningTimer = new(0f);
    private FryingReceipeSO fryingReceipeSO;
    private BurningReceipeSO burningReceipeSO;

    public override void OnNetworkSpawn()
    {
        fryTimer.OnValueChanged += FryingTimer_OnValueChanged;
        burningTimer.OnValueChanged += BurningTimer_OnValueChanged;
        currentState.OnValueChanged += CurrentState_OnValueChanged;
    }
    private void FryingTimer_OnValueChanged(float previousValue, float newValue)
    {
        float fryingTimerMax = fryingReceipeSO != null ? fryingReceipeSO.fryingTimerMax : 1f;
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = fryTimer.Value / fryingTimerMax
        });
    }
    private void BurningTimer_OnValueChanged(float previousValue, float newValue)
    {
        float burningTimerMax = burningReceipeSO != null ? burningReceipeSO.burningTimerMax : 1f;
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = burningTimer.Value / burningTimerMax
        });
    }
    private void CurrentState_OnValueChanged(State previousState, State newState)
    {
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
        {
            state = currentState.Value
        });

        if (currentState.Value == State.Burned || currentState.Value == State.Idle)
        {
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = 0f
            });
        }
    }
    private void Update()
    {
        if (!IsServer) return;

        if (HasKitchenObject())
        {
            switch (currentState.Value)
            {
                case State.Idle:
                    break;

                case State.Frying:
                    fryTimer.Value += Time.deltaTime;

                    if (fryTimer.Value > fryingReceipeSO.fryingTimerMax)
                    {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        KitchenObject.SpawnKitchenObject(fryingReceipeSO.output, this);

                        currentState.Value = State.Fried;
                        burningTimer.Value = 0f;
                        SetBurningReceipeSOClientRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(GetKitchenObject().GetKitchenObjectSO()));
                    }
                    break;

                case State.Fried:
                    burningTimer.Value += Time.deltaTime;

                    if (burningTimer.Value > burningReceipeSO.burningTimerMax)
                    {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        KitchenObject.SpawnKitchenObject(burningReceipeSO.output, this);
                        currentState.Value = State.Burned;
                    }
                    break;

                case State.Burned:
                    break;
            }
        }
    }
    public override void Interact(Player player)
    {
        if (!HasKitchenObject()) // There is no kitchen Object here
        {
            if (player.HasKitchenObject()) // Player is carrying something
            {
                if (HasReceipeInput(player.GetKitchenObject().GetKitchenObjectSO())) // Player Carrying something can be fried
                {
                    KitchenObject kitchenObject = player.GetKitchenObject();
                    kitchenObject.SetKitchenObjectParent(this);

                    InteractLogicPlaceObjectOnStoveCounterServerRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObject.GetKitchenObjectSO()));
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
                        GetKitchenObject().DestroySelf();

                        currentState.Value = State.Idle;
                    }
                }
            }
            else // Player is carrying nothing
            {
                GetKitchenObject().SetKitchenObjectParent(player);
                SetStateIdleServerRpc();
            }
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void SetStateIdleServerRpc()
    {
        currentState.Value = State.Idle;
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void InteractLogicPlaceObjectOnStoveCounterServerRpc(int kitchenObjectSOIndex)
    {
        fryTimer.Value = 0f;
        currentState.Value = State.Frying;
        SetFryingReceipeSOClientRpc(kitchenObjectSOIndex);
    }

    [ClientRpc]
    private void SetFryingReceipeSOClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        fryingReceipeSO = GetFryingReceipeSOInput(kitchenObjectSO);
    }
    [ClientRpc]
    private void SetBurningReceipeSOClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        burningReceipeSO = GetBurningReceipeSOInput(kitchenObjectSO);
    }
    private bool HasReceipeInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingReceipeSO fryingReceipeSO = GetFryingReceipeSOInput(inputKitchenObjectSO);
        return fryingReceipeSO != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingReceipeSO fryingReceipeSO = GetFryingReceipeSOInput(inputKitchenObjectSO);

        if (fryingReceipeSO != null)
        {
            return fryingReceipeSO.output;
        }
        else
        {
            return null;
        }
    }

    private FryingReceipeSO GetFryingReceipeSOInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (FryingReceipeSO fryingReceipeSO in fryingReceipeSOArray)
        {
            if (fryingReceipeSO.input == inputKitchenObjectSO)
            {
                return fryingReceipeSO;
            }
        }
        return null;
    }
    private BurningReceipeSO GetBurningReceipeSOInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (BurningReceipeSO burningReceipeSO in burningReceipeSOArray)
        {
            if (burningReceipeSO.input == inputKitchenObjectSO)
            {
                return burningReceipeSO;
            }
        }
        return null;
    }

    public bool isFried()
    {
        return currentState.Value == State.Fried;
    }
}
