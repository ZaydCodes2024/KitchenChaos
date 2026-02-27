using System;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress
{
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

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

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = fryTimer/ fryingReceipeSO.fryingTimerMax
                    });

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

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = burningTimer/ burningReceipeSO.burningTimerMax
                    });

                    if (burningTimer > burningReceipeSO.burningTimerMax)
                    {
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(burningReceipeSO.output,this);
                        currentState = State.Burned;

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = currentState
                        });

                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            progressNormalized = 0f
                        });
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
                    player.GetKitchenObject().SetKitchenObjectParent(this);

                    fryingReceipeSO = GetFryingReceipeSOInput(GetKitchenObject().GetKitchenObjectSO());

                    currentState = State.Frying;
                    
                    fryTimer = 0f;

                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                    {
                        state = currentState
                    });

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = fryTimer/ fryingReceipeSO.fryingTimerMax
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
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) // Kitchen Object is a plate
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestroySelf();
                        
                        currentState = State.Idle;

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = currentState
                        });

                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            progressNormalized = 0f
                        });
                    }
                }
            }
            else // Player is carrying nothing
            {
                GetKitchenObject().SetKitchenObjectParent(player);
                currentState = State.Idle;

                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                {
                    state = currentState
                });

                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = 0f
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

    public bool isFried()
    {
        return currentState == State.Fried;
    }
}
