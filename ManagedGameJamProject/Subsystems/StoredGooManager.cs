using ManagedGameJamProject.Components;
using UnrealSharp;
using UnrealSharp.Attributes;
using UnrealSharp.UnrealSharpCore;

namespace ManagedGameJamProject.Subsystems;

[UMultiDelegate]
public delegate void FOnGooStorageInventoryChanged(float newValue, float oldValue);

[UClass]
public partial class UStoredGooManager : UCSWorldSubsystem
{
    public override bool DoesSupportWorldType(ECSWorldType worldType)
    {
        return worldType == ECSWorldType.Game || worldType == ECSWorldType.PIE;
    }
    
    [UProperty(PropertyFlags.BlueprintAssignable)]
    public partial TMulticastDelegate<FOnGooStorageInventoryChanged> OnGooStorageInventoryChanged { get; set; }
    
    [UProperty]
    private partial IList<UBKSlimeInventory> GooStorageInventories { get; set; }
    
    private float _lastTotalGooAmount;
    
    public void RegisterGooStorageInventory(UBKSlimeInventory inventory)
    {
        if (!GooStorageInventories.Contains(inventory))
        {
            GooStorageInventories.Add(inventory);
            inventory.OnAttributeChanged += OnAnyGooStorageInventoryChanged;
            OnAnyGooStorageInventoryChanged(null, GetTotalStoredGooAmount(), _lastTotalGooAmount);
        }
    }
    
    public bool DeductSlime(float amount)
    {
        float remainingAmount = amount;
        
        if (GetTotalStoredGooAmount() < amount)
        {
            return false;
        }
        
        foreach (var inventory in GooStorageInventories)
        {
            if (remainingAmount <= 0)
            {
                break;
            }
            
            float availableGoo = inventory.CurrentValue;
            float deduction = Math.Min(availableGoo, remainingAmount);
            inventory.AddSlime(-deduction);
            remainingAmount -= deduction;
        }
        
        return remainingAmount <= 0;
    }
    
    public float GetTotalStoredGooAmount()
    {
        float totalGoo = 0;
        
        foreach (var inventory in GooStorageInventories)
        {
            totalGoo += inventory.CurrentValue;
        }

        return totalGoo;
    }
    
    [UFunction]
    void OnAnyGooStorageInventoryChanged(UAttributeComponent attributeComponent, float newValue, float oldValue)
    {
        float totalGoo = GetTotalStoredGooAmount();
        OnGooStorageInventoryChanged.Invoke(totalGoo, _lastTotalGooAmount);
        _lastTotalGooAmount = totalGoo;
    }
}