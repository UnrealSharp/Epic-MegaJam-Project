using UnrealSharp;
using UnrealSharp.Attributes;
using UnrealSharp.Engine;

namespace ManagedGameJamProject.Components;

[UMultiDelegate]
public delegate void FOnAttributeChanged(UAttributeComponent attributeComponent, float newValue, float oldValue);

[UClass]
public partial class UAttributeComponent : UActorComponent
{
    [UProperty(PropertyFlags.EditAnywhere | PropertyFlags.BlueprintReadWrite)]
    public partial float MaxValue { get; set; }

    [UProperty(PropertyFlags.EditAnywhere | PropertyFlags.BlueprintReadWrite)]
    public partial float CurrentValue { get; set; }

    [UProperty(PropertyFlags.BlueprintAssignable)]
    public partial TMulticastDelegate<FOnAttributeChanged> OnAttributeChanged { get; set; }
    
    public void SetMaxValue(float newMax)
    {
        MaxValue = newMax;
        SetCurrentValue(CurrentValue);
    }

    public void SetCurrentValue(float newAmount)
    {
        float oldValue = CurrentValue;
        CurrentValue = Math.Clamp(newAmount, 0, MaxValue);
        OnAttributeChanged.Invoke(this, CurrentValue, oldValue);
    }
}