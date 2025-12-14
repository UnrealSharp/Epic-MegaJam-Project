using ManagedGameJamProject.Components;
using UnrealSharp;
using UnrealSharp.Attributes;
using UnrealSharp.GameplayTags;

namespace ManagedGameJamProject;

[UMultiDelegate]
public delegate void FOnDeathDelegate(UBKHealthComponent healthComp);

[UClass]
public partial class UBKHealthComponent : UAttributeComponent
{
    [UProperty(PropertyFlags.BlueprintAssignable)]
    public partial TMulticastDelegate<FOnDeathDelegate> OnDeathDelegate { get; set; }
    
    public bool IsDead { get; private set; }

    public override void BeginPlay()
    {
        base.BeginPlay();
        
        UStatsComponent statsComp = Owner.GetComponentByClass<UStatsComponent>();
        if (statsComp == null)
        {
            return;
        }
        statsComp.OnStatsChanged += OnStatsChanged;
    }
    
    [UFunction]
    public void OnStatsChanged(FGameplayTag statTag, float newValue, float oldValue)
    {
        if (statTag != GameplayTags.Game_Stats_MaxHealth)
        {
            return;
        }
        
        float delta = newValue - oldValue;
        SetMaxValue(MaxValue + delta);
        SetCurrentValue(CurrentValue + delta);
    }

    void OnDeath()
    {
        if (IsDead)
        {
            return;
        }
        
        OnDeathDelegate.Invoke(this);
        IsDead = true;
    }
    
    [UFunction(FunctionFlags.BlueprintCallable)]
    public void ApplyDamage(float damageAmount)
    {
        if (damageAmount <= 0)
        {
            return;
        }
        
        SetCurrentValue(CurrentValue - damageAmount);
            
        if (CurrentValue <= 0)
        {
            OnDeath();
        }
    }
}