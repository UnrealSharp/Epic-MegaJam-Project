using ManagedGameJamProject.Components;
using UnrealSharp.Attributes;
using UnrealSharp.Core.Attributes;
using UnrealSharp.Engine;
using UnrealSharp.UMG;

namespace ManagedGameJamProject.Widgets;

[UClass]
public partial class UPlayerHUD : UUserWidget
{
    [UProperty, BindWidget]
    public partial UAttributeCounter SlimeCounter { get; set; }
    
    [UProperty, BindWidget]
    public partial UAttributeCounter HealthCounter { get; set; }
    
    [UProperty,BindWidget]
    public partial UDamageVignette DamageVignette { get; set; }
    
    public void InitializeFrom(AActor character)
    {
        UBKSlimeInventory slimeInventory = character.GetComponentByClass<UBKSlimeInventory>();
        UBKHealthComponent healthComponent = character.GetComponentByClass<UBKHealthComponent>();
        
        SlimeCounter.InitializeFrom(slimeInventory);
        HealthCounter.InitializeFrom(healthComponent);
        DamageVignette.InitializeFrom(healthComponent);
    }
}