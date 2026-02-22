using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent
{
    public static Player Instance { get; private set; }
    
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }
    [SerializeField] private float moveSpeed;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask counterLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;
    private KitchenObject kitchenObject;
    private BaseCounter selectedCounter;
    private float interactDistance = 2f;
    private bool isWalking;
    private float rotateSpeed = 10f;
    private Vector3 lastInteractDir;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one Player instance");
        }
        Instance = this;
    }
    private void Start()
    {
        gameInput.OnInteractAction +=  GameInput_OnInteractAction;
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        selectedCounter?.Interact(this);
    }

    private void Update()
    {
        HandleMovement();
        HandleInteractions();
    }
    
    private void HandleInteractions()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
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
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = 0.7f;
        float playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);
        
        if (!canMove) // Cannot move towards moveDir
        {
            Vector3 moveDirX = new Vector3(moveDir.x,0,0).normalized; // Attempt only X movement
            canMove = moveDir.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

            if (canMove)
            {
                moveDir = moveDirX; // Can move only on the X
            }
            else // Cannot move only on the X
            {
                Vector3 moveDirZ = new Vector3(0,0,moveDir.z).normalized; // Attempt only Z movement
                canMove =  moveDir.z != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

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
