using ManagedGameJamProject.Interaction;
using UnrealSharp;
using UnrealSharp.Attributes;
using UnrealSharp.Engine;
using UnrealSharp.UMG;

namespace ManagedGameJamProject.Shop;

[UClass]
public partial class UOpenShopWidgetInteraction : UInteraction
{
    [UProperty(PropertyFlags.EditAnywhere | PropertyFlags.BlueprintReadOnly)]
    public partial TSubclassOf<UUserWidget> ShopWidgetClass { get; set; }
    
    public override void OnInteract(AActor interactor)
    {
        base.OnInteract(interactor);
        UUserWidget shopWidget = CreateWidget(ShopWidgetClass);
        shopWidget.AddToViewport();
    }
}

[UClass]
public partial class AShopActor : AActor
{
    [UProperty(DefaultComponent = true, RootComponent = true)]
    public partial UStaticMeshComponent Mesh { get; set; }
    
    [UProperty(DefaultComponent = true)]
    public partial UInteractionComponent InteractionComponent { get; set; }
}