using ManagedGameJamProject.Visuals;
using UnrealSharp;
using UnrealSharp.AIModule;
using UnrealSharp.Attributes;
using UnrealSharp.CoreUObject;
using UnrealSharp.Engine;

namespace ManagedGameJamProject.AI.Tasks;

[UClass]
public partial class ULaunchAgainstTarget : UBTTask_BlueprintBase
{
    [UProperty(PropertyFlags.EditInstanceOnly)]
    public partial FBlackboardKeySelector Target { get; set; }
    
    [UProperty(PropertyFlags.EditInstanceOnly)]
    public partial float LocationRadius { get; set; }
    
    [UProperty(PropertyFlags.EditInstanceOnly)]
    public partial float DamageRadius { get; set; }
    
    [UProperty(PropertyFlags.EditInstanceOnly)]
    public partial TSubclassOf<ADangerZoneActor> LandingDangerActorClass { get; set; }
    
    [UProperty(PropertyFlags.EditInstanceOnly)]
    public partial FBlackboardKeySelector TargetLocation { get; set; }
    
    [UProperty(PropertyFlags.EditInstanceOnly)]
    public partial FBlackboardKeySelector RadiusKeySelector { get; set; }
    
    [UProperty]
    public partial ADangerZoneActor LandingDangerActor { get; set; }
    private APawn _controlledPawn;
    
    public override void ReceiveExecuteAI(AAIController ownerController, APawn controlledPawn)
    {
        base.ReceiveExecuteAI(ownerController, controlledPawn);
        
        AActor targetActor = UBTFunctionLibrary.GetBlackboardValueAsActor(this, Target);
        ACharacter target = (ACharacter) targetActor;
        
        FVector velocity = target.CharacterMovement.Velocity;
        FVector endLocation = targetActor.ActorLocation + velocity * 2f;
        
        FVector randomOffset = MathLibrary.RandomUnitVector() * MathLibrary.RandomFloatInRange(0, LocationRadius);
        endLocation += randomOffset;
        
        FTransform spawnTransform = new FTransform();
        spawnTransform.Location = endLocation + -FVector.Up * 50;
        spawnTransform.Rotation = FRotator.ZeroRotator;
        spawnTransform.Scale = FVector.One;
        
        LandingDangerActor = SpawnActor(LandingDangerActorClass, spawnTransform, ESpawnActorCollisionHandlingMethod.AlwaysSpawn);
        float damageRadius = (float)(DamageRadius * controlledPawn.ActorScale3D.X);
        LandingDangerActor.SetDangerZoneSize(damageRadius);
        
        ACharacter character = (ACharacter) controlledPawn;
        
        float scaledCapsuleSize = character.CapsuleComponent.ScaledCapsuleRadius;
        FVector normalizedLaunchDirection = FVector.Normalize(endLocation - controlledPawn.ActorLocation);
        FVector launchStartLocation = controlledPawn.ActorLocation + normalizedLaunchDirection * -scaledCapsuleSize;
        
        UGameplayStatics.SuggestProjectileVelocityCustomArc(out FVector outLaunchVelocity, launchStartLocation, endLocation);
        
        UBTFunctionLibrary.SetBlackboardValueAsVector(this, TargetLocation, endLocation);
        UBTFunctionLibrary.SetBlackboardValueAsFloat(this, RadiusKeySelector, damageRadius);
        
        character.LaunchCharacter(outLaunchVelocity, true, true);
        character.CapsuleComponent.SetCollisionResponseToChannel(UnrealSharp.Engine.ECollisionChannel.ECC_Pawn, ECollisionResponse.ECR_Ignore);
        character.LandedDelegate += OnLaunchFinished;
        
        FRotator lookAtRotation = MathLibrary.FindLookAtRotation(controlledPawn.ActorLocation, endLocation);
        controlledPawn.SetActorRotation(lookAtRotation);
        
        _controlledPawn = controlledPawn;
    }
    
    [UFunction]
    public void OnLaunchFinished(FHitResult hit)
    {
        if (_controlledPawn.IsValid())
        {
            ACharacter character = (ACharacter) _controlledPawn;
            character.CapsuleComponent.SetCollisionResponseToChannel(UnrealSharp.Engine.ECollisionChannel.ECC_Pawn, ECollisionResponse.ECR_Block);
            character.LandedDelegate -= OnLaunchFinished;
        }
        
        if (LandingDangerActor.IsValid())
        {
            LandingDangerActor.DestroyActor();
        }
        
        FinishExecute(true);
    }

    public override void ReceiveAbort(AActor ownerActor)
    {
        base.ReceiveAbort(ownerActor); 
        
        if (LandingDangerActor.IsValid())
        {
            LandingDangerActor.DestroyActor();
        }
    }
}