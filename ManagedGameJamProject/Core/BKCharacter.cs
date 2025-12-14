using ManagedGameJamProject.Buffs;
using ManagedGameJamProject.Components;
using ManagedGameJamProject.Equipment;
using ManagedGameJamProject.Interaction;
using ManagedGameJamProject.Widgets;
using UnrealSharp;
using UnrealSharp.Attributes;
using UnrealSharp.Engine;
using UnrealSharp.EnhancedInput;
using UnrealSharp.UMG;

namespace ManagedGameJamProject.Core;

[UClass]
public partial class ABKCharacter : ABKCharacterBase
{
    [UProperty(DefaultComponent = true)]
    public partial UBKSlimeInventory SlimeInventory { get; set; }
    
    [UProperty(DefaultComponent = true)]
    public partial UInteractionChecker InteractionChecker { get; set; }
    
    [UProperty(PropertyFlags.EditDefaultsOnly)]
    public partial TSubclassOf<UEndGameWidget> EndGameWidgetClass { get; set; }
    
    [UProperty(DefaultComponent = true)]
    public partial UEquipmentComponent EquipmentComponent { get; set; }
    
    [UProperty(DefaultComponent = true)]
    public partial UBuffComponent BuffComponent { get; set; }
    
    [UProperty(DefaultComponent = true)]
    public partial UStatsComponent StatsComponent { get; set; }
    
    [UProperty(PropertyFlags.EditDefaultsOnly)]
    public partial UInputAction OpenEscapeMenuAction { get; set; }
    
    [UProperty]
    private partial UBKSlimeLevelManager SlimeLevelManager { get; set; }

    public override void BeginPlay()
    {
        base.BeginPlay();
        
        SlimeLevelManager = NewObject<UBKSlimeLevelManager>(this);
        SlimeLevelManager.InitializeFrom(SlimeInventory, Mesh, 5);
        SlimeInventory.SetMaxValue(100.0f);
        SlimeInventory.SetCurrentValue(0.0f);
        
        UEnhancedInputComponent inputComponent = (UEnhancedInputComponent)InputComponent;
        inputComponent.BindAction(OpenEscapeMenuAction, ETriggerEvent.Completed, Callback);
        
        HurtComponent.InitializeFrom(Mesh);
    }

    public override void Possessed(AController newController)
    {
        base.Possessed(newController);
        
        APlayerController playerController = (APlayerController)newController;
        WidgetLibrary.SetInputModeGameAndUI(playerController, null, EMouseLockMode.LockOnCapture, false);
    }

    [UFunction]
    private void Callback(FInputActionValue arg1, float arg2, float arg3, UInputAction arg4)
    {
        CreateEscapeMenu(true);
    }

    public override void OnDeath(UBKHealthComponent healthComp)
    {
        base.OnDeath(healthComp);
        CreateEscapeMenu(false);
    }
    
    void CreateEscapeMenu(bool isPauseMenu)
    {
        UEndGameWidget endGameWidget = CreateWidget(EndGameWidgetClass);
        endGameWidget.InitializeFrom(isPauseMenu);
        endGameWidget.AddToViewport();
    }
}