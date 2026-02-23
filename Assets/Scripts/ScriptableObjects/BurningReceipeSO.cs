using UnityEngine;

[CreateAssetMenu()]
public class BurningReceipeSO : ScriptableObject
{
     public KitchenObjectSO input;
    public KitchenObjectSO output;
    public float burningTimerMax;
}
