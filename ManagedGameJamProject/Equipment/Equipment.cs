using ManagedGameJamProject.Components;
using UnrealSharp.Attributes;
using UnrealSharp.CoreUObject;
using UnrealSharp.Engine;
using UnrealSharp.GameplayTags;

namespace ManagedGameJamProject.Equipment;

[UClass]
public partial class UEquipment : UObject
{
    protected float IntervalBetweenAttacks;
    protected FGameplayTag SpeedStatTag;
    
    private FTimerHandle _attackTimerHandle;
    protected AActor? Owner;
    
    [UProperty(PropertyFlags.EditDefaultsOnly | PropertyFlags.BlueprintReadOnly)]
    public partial UAnimMontage Montage { get; set; }
    
    [UProperty(PropertyFlags.EditDefaultsOnly)]
    public partial USoundBase EquipmentSound { get; set; }
    
    public virtual void InitializeFromOwner(AActor character)
    {
        Owner = character;
    }
    
    public void Equip()
    {
        OnEquip();
        
        UStatsComponent statsComponent = Owner!.GetComponentByClass<UStatsComponent>();
        
        float interval = IntervalBetweenAttacks;
        if (statsComponent.TryGetStat(SpeedStatTag, out float percentageBonus))
        {
            interval *= (1.0f - percentageBonus);
        }
        
        _attackTimerHandle = SystemLibrary.SetTimer(Use, interval, true);
    }
    
    public void UnEquip()
    {
        OnUnEquip();
        SystemLibrary.ClearAndInvalidateTimerHandle(ref _attackTimerHandle);
    }
    
    protected virtual void OnEquip()
    {
        
    }
    
    protected virtual void OnUnEquip()
    {
        
    }

    [UFunction]
    void Use()
    {
        if (!CanUse())
        {
            return;
        }
        
        OnUse();
    }
    
    protected virtual bool CanUse()
    {
        return true;
    }
    
    protected virtual void OnUse()
    {
        
    }
    
    public virtual void TickEquipment(float deltaTime)
    {
        
    }
}