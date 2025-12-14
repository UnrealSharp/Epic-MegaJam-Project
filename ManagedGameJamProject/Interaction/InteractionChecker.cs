using ManagedGameJamProject.Widgets;
using UnrealSharp;
using UnrealSharp.Attributes;
using UnrealSharp.CoreUObject;
using UnrealSharp.Engine;
using UnrealSharp.UMG;

namespace ManagedGameJamProject.Interaction;

[UClass]
public partial class UInteractionChecker : UActorComponent
{
    public UInteractionChecker()
    {
       InteractionRadius = 200.0f;
    }
    
    [UProperty(PropertyFlags.EditDefaultsOnly)]
    public partial float InteractionRadius { get; set; }
    
    [UProperty(PropertyFlags.EditDefaultsOnly)]
    public partial TSubclassOf<UInteractionWidget> InteractionWidgetClass { get; set; }
    
    [UProperty]
    public partial UInteractionWidget InteractionWidget { get; set; }

    private readonly List<AActor> _actorsToIgnore = new();
    private UInteractionComponent? _currentInteractionComp;

    public override void BeginPlay()
    {
        base.BeginPlay();
        
        if (!InteractionWidgetClass.IsValid)
        {
            LogBK.LogWarning("InteractionChecker: InteractionWidgetClass is not set.");
            return;
        }
        
        InteractionWidget = CreateWidget(InteractionWidgetClass);
        InteractionWidget.AddToViewport();
        InteractionWidget.Visibility = ESlateVisibility.Hidden;
        _actorsToIgnore.Add(Owner);
        
        SystemLibrary.SetTimer(CheckForInteractions, 0.01f, true);
    }
    
    [UFunction]
    void CheckForInteractions()
    {
        FVector start = Owner.ActorLocation;
        if (!SystemLibrary.MultiSphereTraceByChannel(start, start, InteractionRadius,
                ETraceChannel.Visibility.ToQuery(),
                false,
                _actorsToIgnore,
                EDrawDebugTrace.None,
                out IList<FHitResult> outHits,
                true))
        {
            return;
        }
        
        double closestDistance = double.MaxValue;
        UInteractionComponent? closestInteraction = null;
        
        foreach (FHitResult hit in outHits)
        {
            AActor hitActor = hit.Actor;
            if (hitActor == null)
            {
                continue;
            }
            
            UInteractionComponent interactionComp = hitActor.GetComponentByClass<UInteractionComponent>();
            if (interactionComp == null)
            {
                continue;
            }
             
            double distance = FVector.Distance(Owner.ActorLocation, hitActor.ActorLocation);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestInteraction = interactionComp;
            }
        }
        
        if (Equals(closestInteraction, _currentInteractionComp))
        {
            return;
        }
        
        _currentInteractionComp = closestInteraction!;      
        
        if (_currentInteractionComp != null)
        {
            InteractionWidget.InitializeFrom(_currentInteractionComp);
            InteractionWidget.Visibility = ESlateVisibility.Visible;
        }
        else
        {
            InteractionWidget.Visibility = ESlateVisibility.Hidden;
        }
    }
    
    [UFunction(FunctionFlags.BlueprintCallable)]
    public void TryPerformInteraction()
    {
        if (_currentInteractionComp != null)
        {
            _currentInteractionComp.Interact(Owner);
        }
    }

    [UFunction(FunctionFlags.BlueprintCallable)]
    public void EndInteraction()
    {
        if (_currentInteractionComp != null)
        {
            _currentInteractionComp.EndInteract(Owner);
        }
    }
}