using UnityEngine;

[CreateAssetMenu()]
public class FryingReceipeSO : ScriptableObject
{
    public KitchenObjectSO input;
    public KitchenObjectSO output;
    public float fryingTimerMax;
}
