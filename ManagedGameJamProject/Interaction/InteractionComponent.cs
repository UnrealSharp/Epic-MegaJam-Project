using UnrealSharp.Attributes;
using UnrealSharp.Engine;

namespace ManagedGameJamProject.Interaction;

[UClass]
public partial class UInteractionComponent : UActorComponent
{
    [UProperty(PropertyFlags.EditAnywhere)] 
    public partial UInteractionInfo? InteractionInfo { get; set; }
    
    [UProperty(PropertyFlags.BlueprintReadOnly)]
    public partial UInteraction? Interaction { get; set; }

    public override void BeginPlay()
    {
        base.BeginPlay();

        if (InteractionInfo == null)
        {
            LogBK.LogWarning("InteractionComponent: No InteractionInfo set on InteractionComponent.");
            return;
        }
        
        Interaction = NewObject(this, InteractionInfo.InteractionClass);
        Interaction.InitializeFrom(this);
    }

    [UFunction(FunctionFlags.BlueprintCallable)]
    public void Interact(AActor interactingActor)
    {
        if (Interaction == null)
        {
            LogBK.LogWarning("InteractionComponent: No Interaction instance found on Interact call.");
            return;
        }
        
        Interaction.OnInteract(interactingActor);
    }

    [UFunction(FunctionFlags.BlueprintCallable)]
    public void EndInteract(AActor interactingActor)
    {
        if (Interaction == null)
        {
            LogBK.LogWarning("InteractionComponent: No Interaction instance found on EndInteract call.");
            return;
        }
        
        Interaction.OnEndInteract(interactingActor);
    }
}