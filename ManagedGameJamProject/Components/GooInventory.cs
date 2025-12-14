using UnrealSharp.Attributes;

namespace ManagedGameJamProject.Components;

[UMultiDelegate]
public delegate void FOnSlimeAmountChanged(UBKSlimeInventory inventory, int NewAmount);

[UClass]
public partial class UBKSlimeInventory : UAttributeComponent
{
    [UFunction(FunctionFlags.BlueprintCallable)]
    public float AddSlime(float amount)
    {
        float newValue = Math.Clamp(CurrentValue + amount, 0, MaxValue);
        float actualAdded = Math.Abs(newValue - CurrentValue);
        
        if (actualAdded == 0)
        {
            return 0;
        }
        
        SetCurrentValue(newValue);
        return actualAdded;
    }
    
    [UFunction(FunctionFlags.BlueprintCallable)]
    public float FlushSlime()
    {
        float temp = CurrentValue;
        SetCurrentValue(0);
        return temp;
    }
    
    [UFunction(FunctionFlags.BlueprintCallable)]
    public float Transfer(UBKSlimeInventory otherInventory)
    {
        float spaceAvailable = otherInventory.MaxValue - otherInventory.CurrentValue;
        float amountToTransfer = Math.Min(CurrentValue, spaceAvailable);

        if (amountToTransfer <= 0)
        {
            return amountToTransfer;
        }
        
        SetCurrentValue(CurrentValue - amountToTransfer);
        otherInventory.AddSlime(amountToTransfer);

        return amountToTransfer;
    }
    
    [UFunction(FunctionFlags.BlueprintCallable)]
    public bool IsFull()
    {
        return CurrentValue >= MaxValue;
    }
}