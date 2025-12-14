using ManagedGameJamProject.Widgets;
using UnrealSharp;
using UnrealSharp.Attributes;
using UnrealSharp.Engine;

namespace ManagedGameJamProject.Core;

[UClass]
public partial class ABKHUD : AHUD
{
    [UProperty(PropertyFlags.EditDefaultsOnly)]
    public partial TSubclassOf<UPlayerHUD> PlayerHUDClass { get; set; }
    
    public override void BeginPlay()
    {
        base.BeginPlay();
        
        if (!PlayerHUDClass.IsValid)
        {
            LogBK.LogWarning("PlayerHUDClass is not set in BKHUD");
            return;
        }
        
        UPlayerHUD playerHud = CreateWidget(PlayerHUDClass, OwningPlayerController);
        playerHud.InitializeFrom(OwningPawn);
        playerHud.AddToViewport();
    }
}