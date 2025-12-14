using UnrealSharp.Attributes;
using UnrealSharp.CoreUObject;
using UnrealSharp.Engine;

namespace ManagedGameJamProject.Components;

[UClass]
public partial class UBKSlimeLevelManager : UObject
{
    [UProperty]
    partial UMaterialInstanceDynamic SlimeMaterialInstance { get; set; }
    
    [UProperty]
    private partial UBKSlimeInventory SlimeInventory { get; set; }

    public void InitializeFrom(UBKSlimeInventory inventory, UPrimitiveComponent mesh, int index = 0)
    {
        SlimeInventory = inventory;
        SlimeMaterialInstance = mesh.CreateDynamicMaterialInstance(index, mesh.GetMaterial(index));
        mesh.SetMaterial(index, SlimeMaterialInstance);
        
        SlimeInventory.OnAttributeChanged += OnSlimeAmountChanged;
        OnSlimeAmountChanged(SlimeInventory, SlimeInventory.CurrentValue, SlimeInventory.CurrentValue);
    }
    
    [UFunction]
    private void OnSlimeAmountChanged(UAttributeComponent slimeInventory, float newAmount, float oldAmount)
    {
        float percentFull = newAmount / slimeInventory.MaxValue;
        SlimeMaterialInstance.SetScalarParameterValue("SlimeAmount",  percentFull);
    }
}