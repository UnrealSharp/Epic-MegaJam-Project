using ManagedGameJamProject.Components;
using UnrealSharp.Attributes;
using UnrealSharp.CoreUObject;
using UnrealSharp.Engine;

namespace ManagedGameJamProject.AI;

[UClass]
public partial class UBKGrowComponent : UActorComponent
{
    UBKHealthComponent healthComponent;
    
    public override void BeginPlay()
    {
        base.BeginPlay();
        
        healthComponent = Owner.GetComponentByClass<UBKHealthComponent>();
        
        if (healthComponent == null)
        {
            LogBK.LogError("BKGrowComponent missing required components");
            return;
        }
        
        healthComponent.OnAttributeChanged += OnHealthChanged;
        AdjustScale(healthComponent.CurrentValue);
    }
    
    [UFunction]
    void OnHealthChanged(UAttributeComponent healthComp, float newValue, float oldValue)
    {
        AdjustScale(newValue);
    }
    
    void AdjustScale(float newValue)
    {
        Owner.ActorScale3D = new FVector(1.0f, 1.0f, 1.0f) * (1.0f + (newValue / 10.0f));
    }
}