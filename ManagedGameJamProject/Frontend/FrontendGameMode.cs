using UnrealSharp;
using UnrealSharp.Attributes;
using UnrealSharp.Engine;
using UnrealSharp.UMG;

namespace ManagedGameJamProject.Frontend;

[UClass]
public partial class AFrontendGameMode : AGameMode
{
    [UProperty(PropertyFlags.EditAnywhere)]
    public partial TSubclassOf<UUserWidget> FrontendWidgetClass { get; set; }

    public override void BeginPlay()
    {
        base.BeginPlay();
        
        UUserWidget frontendWidget = CreateWidget(FrontendWidgetClass);
        frontendWidget.AddToViewport();
    }
}