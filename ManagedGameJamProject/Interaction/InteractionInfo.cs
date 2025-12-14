using UnrealSharp;
using UnrealSharp.Attributes;
using UnrealSharp.Core;
using UnrealSharp.CoreUObject;
using UnrealSharp.Engine;
using UnrealSharp.EnhancedInput;
using UnrealSharp.UnrealSharpCore;

namespace ManagedGameJamProject.Interaction;

[UClass]
public partial class UInteraction : UObject
{
    [UProperty]
    protected partial UInteractionComponent? OwningInteractionComponent { get; set; }
    
    public void InitializeFrom(UInteractionComponent owner)
    {
        if (OwningInteractionComponent != null)
        {
            return;
        }
        
        OwningInteractionComponent = owner;
        OnInitialize();
    }

    protected virtual void OnInitialize()
    {
        
    }
    
    public virtual bool CanInteract(AActor interactor)
    {
        return true;
    }
    
    public virtual void OnInteract(AActor interactor)
    {
        
    }
    
    public virtual void OnEndInteract(AActor interactor)
    {
        
    }
    
    protected T? GetOuterActorAs<T>() where T : AActor
    {
        if (OwningInteractionComponent == null)
        {
            return null;
        }
        
        return (T) OwningInteractionComponent.Owner;
    }
}

[UClass]
public partial class UInteractionInfo : UCSPrimaryDataAsset
{
    [UProperty(PropertyFlags.EditDefaultsOnly)]
    public partial FText InteractionName { get; set; }
    
    [UProperty(PropertyFlags.EditDefaultsOnly)]
    public partial UInputAction InputAction { get; set; }
    
    [UProperty(PropertyFlags.EditDefaultsOnly)]
    public partial float InteractionDistance { get; set; }
    
    [UProperty(PropertyFlags.EditDefaultsOnly)]
    public partial TSubclassOf<UInteraction> InteractionClass { get; set; }
}