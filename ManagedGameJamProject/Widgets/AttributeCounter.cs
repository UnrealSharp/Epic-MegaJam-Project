using ManagedGameJamProject.Components;
using UnrealSharp.Attributes;
using UnrealSharp.Core;
using UnrealSharp.Core.Attributes;
using UnrealSharp.CoreUObject;
using UnrealSharp.SlateCore;
using UnrealSharp.UMG;

namespace ManagedGameJamProject.Widgets;

[UClass]
public partial class UAttributeCounter : UUserWidget
{
    [UProperty, BindWidget]
    public partial UProgressBar AttributeBar { get; set;  }
    
    [UProperty, BindWidget]
    public partial UTextBlock AttributeNameText { get; set; } 
    
    [UProperty(PropertyFlags.EditAnywhere)]
    public partial FSlateColor FillColor { get; set; }
    
    [UProperty(PropertyFlags.Transient), BindWidgetAnim]
    public partial UWidgetAnimation ValueChangeAnim { get; set; }
    
    [UProperty(PropertyFlags.EditAnywhere)]
    public partial FText AttributeName { get; set; } 
    
    private UAttributeComponent? _attributeComponent;
    private bool _isPlayingAnimation = false;

    public override void PreConstruct(bool isDesignTime)
    {
        base.PreConstruct(isDesignTime);
        
        FProgressBarStyle brush = AttributeBar.Style;
        brush.FillImage.Tint = FillColor;
        AttributeBar.Style = brush;
    }

    public void InitializeFrom(UAttributeComponent attributeComponent)
    {
        if (_attributeComponent != null && attributeComponent.IsValid())
        {
            _attributeComponent.OnAttributeChanged -= OnAttributeChanged;
        }
        
        _attributeComponent = attributeComponent;

        attributeComponent.OnAttributeChanged += OnAttributeChanged; 
        OnAttributeChanged(attributeComponent, attributeComponent.CurrentValue, attributeComponent.CurrentValue);
    }
    
    [UFunction]
    public void OnAttributeChanged(UAttributeComponent attributeComponent, float newValue, float oldValue)
    {
        float percent = newValue / attributeComponent.MaxValue;
        
        if (float.IsNaN(percent) || float.IsInfinity(percent))
        {
            percent = 0.0f;
        }
        
        AttributeBar.Percent = percent;
        
        double roundedNewValue = Math.Round(newValue, 2);
        double roundedMaxValue = Math.Round(attributeComponent.MaxValue, 2);
        
        AttributeNameText.Text = AttributeName + $" {roundedNewValue}/{roundedMaxValue}";
        
        if (_isPlayingAnimation)
        {
            return;
        }
        
        PlayAnimation(ValueChangeAnim);
    }

    public override void OnAnimationFinished(UWidgetAnimation animation)
    {
        if (Equals(animation, ValueChangeAnim))
        {
            _isPlayingAnimation = false;
        }
    }
}