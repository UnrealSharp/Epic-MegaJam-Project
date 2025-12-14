using ManagedGameJamProject.Components;
using UnrealSharp.Attributes;
using UnrealSharp.Core.Attributes;
using UnrealSharp.CoreUObject;
using UnrealSharp.UMG;

namespace ManagedGameJamProject.Widgets;

[UClass]
public partial class UDamageVignette : UUserWidget
{
    [UProperty, BindWidget]
    public partial UImage VignetteBorder { get; set; }
    
    [UProperty(PropertyFlags.Transient), BindWidgetAnim]
    public partial UWidgetAnimation DamageVignetteAnim { get; set; }
    
    private UBKHealthComponent? _healthComponent;
    
    public void InitializeFrom(UBKHealthComponent healthComponent)
    {
        if (_healthComponent != null && healthComponent.IsValid())
        {
            _healthComponent.OnAttributeChanged -= OnDamageTaken;
        }
        
        _healthComponent = healthComponent;

        healthComponent.OnAttributeChanged += OnDamageTaken; 
        VignetteBorder.RenderOpacity = 0.0f;
    }
    
    bool _isPlayingAnimation = false;
    
    [UFunction]
    public void OnDamageTaken(UAttributeComponent attributeComponent, float newValue, float oldValue)
    {
        if (_isPlayingAnimation)
        {
            return;
        }
        
        if (newValue >= oldValue)
        {
            return;
        }
        
        PlayAnimation(DamageVignetteAnim);
        _isPlayingAnimation = true;
    }

    public override void OnAnimationFinished(UWidgetAnimation animation)
    {
        if (Equals(animation, DamageVignetteAnim))
        {
            _isPlayingAnimation = false;
        }
    }
}