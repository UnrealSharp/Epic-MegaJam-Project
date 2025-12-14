using UnrealSharp.Attributes;
using UnrealSharp.CoreUObject;
using UnrealSharp.Engine;

namespace ManagedGameJamProject.Buffs;

[UClass]
public partial class UBuff : UObject
{
    protected AActor TargetActor;
    protected float Duration = 5.0f;
    protected UBuffComponent OwningComponent;
    
    public void Initialize(AActor target, UBuffComponent component, bool delayedStart = false)
    {
        TargetActor = target;
        OwningComponent = component;
        
        if (!delayedStart)
        {
            ApplyBuff();
        }
    }
    
    public void ApplyBuff()
    {
        OnApplyBuff(TargetActor);
        SystemLibrary.SetTimer(ExpireBuff, Duration, false);
    }
    
    protected virtual void OnApplyBuff(AActor target)
    {
        
    }
    
    protected virtual void OnRemoveBuff()
    {
        
    }
    
    [UFunction]
    void ExpireBuff()
    {
        OnRemoveBuff();
        OwningComponent.RemoveBuff(this);
    }
}