using System;
using UnityEngine;

public class StoveCounter : BaseCounter
{
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;

    public class OnStateChangedEventArgs: EventArgs
    {
        public State state;
    }
    public enum State
    {
        Idle, Frying, Fried, Burned
    }

    private State currentState;
   [SerializeField] private FryingReceipeSO[] fryingReceipeSOArray;
   [SerializeField] private BurningReceipeSO[] burningReceipeSOArray;
    private float fryTimer;
    private float burningTimer;
    private FryingReceipeSO fryingReceipeSO;
    private BurningReceipeSO burningReceipeSO;

    private void Start()
    {
        currentState = State.Idle;
    }
    private void Update()
    {
        if (HasKitchenObject())
        {
            switch(currentState)
            {
                case State.Idle:
                break;

                case State.Frying:
                    fryTimer += Time.deltaTime;
                    if (fryTimer > fryingReceipeSO.fryingTimerMax)
                    {
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(fryingReceipeSO.output,this);

                        currentState = State.Fried;
                        burningTimer = 0f;
                        burningReceipeSO = GetBurningReceipeSOInput(GetKitchenObject().GetKitchenObjectSO());

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = currentState
                        });
                    }
                break;

                case State.Fried:
                    burningTimer += Time.deltaTime;
                    if (burningTimer > burningReceipeSO.burningTimerMax)
                    {
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(burningReceipeSO.output,this);
                        currentState = State.Burned;

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = currentState
                        });
                    }
                break;

                case State.Burned:
                break;
            }
        }
        Debug.Log(currentState);
    }
    public override void Interact(Player player)
    {
         if (!HasKitchenObject()) // There is no kitchen Object here
        {
            if (player.HasKitchenObject()) // Player is carrying something
            {
                if (HasReceipeInput(player.GetKitchenObject().GetKitchenObjectSO())) // Player Carrying something can be fried
                {
                    player.GetKitchenObject().SetKitchenObjectParent(this);

                    fryingReceipeSO = GetFryingReceipeSOInput(GetKitchenObject().GetKitchenObjectSO());

                    currentState = State.Frying;
                    
                    fryTimer = 0f;

                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                    {
                        state = currentState
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
                currentState = State.Idle;

                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                {
                    state = currentState
                });
            }
        }
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
}
