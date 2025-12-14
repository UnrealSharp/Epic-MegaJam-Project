using UnrealSharp.AIModule;
using UnrealSharp.Attributes;
using UnrealSharp.CoreUObject;
using UnrealSharp.Engine;

namespace ManagedGameJamProject.AI.Tasks;

[UClass]
public partial class UBKMoveToPlayerLocation : UBTTask_BlueprintBase
{
    [UProperty(PropertyFlags.EditAnywhere)]
    partial FBlackboardKeySelector TargetActorKey { get; set; }
    
    public override void ReceiveExecuteAI(AAIController ownerController, APawn controlledPawn)
    {
        base.ReceiveExecuteAI(ownerController, controlledPawn);
        
        APawn playerPawn = UGameplayStatics.GetPlayerPawn(0);
        UAITask_MoveTo taskMoveTo = UAITask_MoveTo.MoveToLocationorActor(ownerController, FVector.Zero, playerPawn);
        taskMoveTo.ReadyForActivation();
            
        FinishExecute(true);
    }

    public override void ReceiveTickAI(AAIController ownerController, APawn controlledPawn, float deltaSeconds)
    {
        base.ReceiveTickAI(ownerController, controlledPawn, deltaSeconds);
        
        UObject targetActor = UBTFunctionLibrary.GetBlackboardValueAsObject(this, TargetActorKey);
        if (targetActor != null)
        {
            FinishExecute(true);
        }
    }
}