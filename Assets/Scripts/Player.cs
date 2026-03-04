using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour, IKitchenObjectParent
{
    public static Player localInstance { get; private set; }
    public static event EventHandler OnAnyPlayerSpawned;
    public static event EventHandler OnAnyPickedSomething;
    public static void ResetStaticData()
    {
        OnAnyPlayerSpawned = null;
    }
    public event EventHandler OnPickedSomething;
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }
    [SerializeField] private float moveSpeed;
    [SerializeField] private LayerMask counterLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;
    [SerializeField] private List<Vector3> spawnPositionList;
    private KitchenObject kitchenObject;
    private BaseCounter selectedCounter;
    private float interactDistance = 2f;
    private bool isWalking;
    private float rotateSpeed = 10f;
    private Vector3 lastInteractDir;

    private void Start()
    {
        GameInput.Instance.OnInteractAction +=  GameInput_OnInteractAction;
        GameInput.Instance.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
    }
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            localInstance = this;
        }

        transform.position = spawnPositionList[(int)OwnerClientId];
        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);

        if (IsServer)   NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        if (clientId == OwnerClientId && HasKitchenObject())
        {
            KitchenObject.DestroyKitchenObject(GetKitchenObject());
        }
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e)
    {
        if (!GameManager.Instance.IsGamePlaying())  return;
        selectedCounter?.InteractAlternate(this);
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if (!GameManager.Instance.IsGamePlaying())  return;
        selectedCounter?.Interact(this);
    }

    private void Update()
    {
        if (!IsOwner)   return;
        
        HandleMovement();
        HandleInteractions();
    }
    
    private void HandleInteractions()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if (moveDir != Vector3.zero)
        {
            lastInteractDir = moveDir;
        }

        if (Physics.Raycast(transform.position,lastInteractDir, out RaycastHit hitinfo, interactDistance ,counterLayerMask))
        {
            if (hitinfo.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                if (baseCounter != selectedCounter)
                {
                    SetSelectedCounter(baseCounter);
                }
            }
            else
            {
                SetSelectedCounter(null);
            }
        }
        else
            {
                SetSelectedCounter(null);
            }
    }
    
    public bool IsWalking()
    {
        return isWalking;
    }

    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = selectedCounter
        });
    }

    private void HandleMovement()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = 0.7f;
        float playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);
        
        if (!canMove) // Cannot move towards moveDir
        {
            Vector3 moveDirX = new Vector3(moveDir.x,0,0).normalized; // Attempt only X movement
            canMove = ( moveDir.x < -0.5f || moveDir.x > 0.5f ) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

            if (canMove)
            {
                moveDir = moveDirX; // Can move only on the X
            }
            else // Cannot move only on the X
            {
                Vector3 moveDirZ = new Vector3(0,0,moveDir.z).normalized; // Attempt only Z movement
                canMove =  ( moveDir.z < -0.5f || moveDir.z > 0.5f ) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

                if (canMove)    
                {
                    moveDir = moveDirZ; // Can move only on the Z
                }
                else // Cannot move in any direction
                {
                    
                }
            }
        }
        if (canMove)
        {
            transform.position += moveDir * moveDistance;
        }

        isWalking = moveDir != Vector3.zero;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, rotateSpeed * Time.deltaTime);
    }

     public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;

        if (kitchenObject != null)
            OnPickedSomething?.Invoke(this, EventArgs.Empty);
            OnAnyPickedSomething?.Invoke(this, EventArgs.Empty);
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

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }
}
