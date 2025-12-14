using UnrealSharp.Attributes;
using UnrealSharp.Engine;

namespace ManagedGameJamProject.Components;

[UClass]
public partial class UHurtComponent : UActorComponent
{
    public UHurtComponent()
    {
        HitEffectDuration = 0.1f;
    }
    
    public UMaterialInstanceDynamic MaterialInstance;
    
    private UAttributeComponent _attributeComponent;
    private UPrimitiveComponent _primitiveComponent;
    private FTimerHandle _resetHitEffectTimer;
    
    [UProperty(PropertyFlags.EditDefaultsOnly)]
    public partial float HitEffectDuration { get; set; }

    public void InitializeFrom(UPrimitiveComponent primitive)
    {
        _attributeComponent = Owner.GetComponentByClass<UBKHealthComponent>();
        if (_attributeComponent == null)
        {
            LogBK.LogWarning("HurtComponent: No AttributeComponent found on owner actor.");
            return;
        }
        
        _attributeComponent.OnAttributeChanged += OnDamageApplied;
        _primitiveComponent = primitive;
        
        MaterialInstance = _primitiveComponent.CreateDynamicMaterialInstance(0, _primitiveComponent.GetMaterial(0));
    }
    
    [UFunction]
    public virtual void OnDamageApplied(UAttributeComponent attributeComp, float newHealth, float oldValue)
    {
        if (newHealth >= oldValue || SystemLibrary.IsTimerActiveHandle(_resetHitEffectTimer))
        {
            return;
        }
        
        MaterialInstance.SetScalarParameterValue("OnHit", 1.0f);
        _resetHitEffectTimer = SystemLibrary.SetTimer(ResetHitEffect, HitEffectDuration, false);
    }
    
    [UFunction]
    public void ResetHitEffect()
    {
        MaterialInstance.SetScalarParameterValue("OnHit", 0.0f);
    }
}