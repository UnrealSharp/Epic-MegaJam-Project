using UnrealSharp;
using UnrealSharp.AIModule;
using UnrealSharp.Attributes;
using UnrealSharp.CoreUObject;
using UnrealSharp.Engine;

namespace ManagedGameJamProject.AI.Tasks;

[UClass]
public partial class UChargeTask : UBTTask_BlueprintBase
{
    [UProperty(PropertyFlags.EditAnywhere | PropertyFlags.BlueprintReadWrite)]
    public partial float ChargeSpeed { get; set; }
    
    [UProperty(PropertyFlags.EditAnywhere | PropertyFlags.BlueprintReadWrite)]
    public partial float ChargeDuration { get; set; }
    
    [UProperty(PropertyFlags.EditAnywhere | PropertyFlags.BlueprintReadWrite)]
    public partial TSubclassOf<AActor> ChargeEffectClass { get; set; }
    
    [UProperty(PropertyFlags.EditAnywhere | PropertyFlags.BlueprintReadWrite)]
    public partial FBlackboardKeySelector TargetKey { get; set; }

    public override async void ReceiveExecuteAI(AAIController ownerController, APawn controlledPawn)
    {
        base.ReceiveExecuteAI(ownerController, controlledPawn);
        
        FTransform spawnTransform = new FTransform();
        spawnTransform.Location = controlledPawn.ActorLocation;
        spawnTransform.Rotation = controlledPawn.ActorRotation;
        spawnTransform.Scale = FVector.One;
        
        AActor target = UBTFunctionLibrary.GetBlackboardValueAsActor(this, TargetKey);
        FVector directionToTarget = FVector.Normalize(target.ActorLocation - controlledPawn.ActorLocation);

        await Task.Delay((int)(ChargeDuration * 1000));
        
        ACharacter ownerCharacter = (ACharacter)controlledPawn;
        ownerCharacter.LaunchCharacter(directionToTarget * ChargeSpeed, true, true);
        FinishExecute(true);
    }
    
}