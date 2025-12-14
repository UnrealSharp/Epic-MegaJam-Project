using ManagedGameJamProject.AI.Projectiles;
using UnrealSharp;
using UnrealSharp.AIModule;
using UnrealSharp.Attributes;
using UnrealSharp.CoreUObject;
using UnrealSharp.Engine;

namespace ManagedGameJamProject.AI.Tasks;

[UClass]
public partial class UShootProjectileTask : UBTTask_BlueprintBase
{
    [UProperty(PropertyFlags.EditAnywhere | PropertyFlags.BlueprintReadOnly)]
    public partial TSubclassOf<APhysicsProjectile> ProjectileClass { get; set; }
    
    [UProperty(PropertyFlags.EditAnywhere | PropertyFlags.BlueprintReadOnly)]
    public partial float ShootingInterval { get; set; }

    public override void ReceiveExecuteAI(AAIController ownerController, APawn controlledPawn)
    {
        base.ReceiveExecuteAI(ownerController, controlledPawn);

        FTransform spawnTransform = new FTransform();
        spawnTransform.Location = controlledPawn.ActorLocation;
        spawnTransform.Rotation = controlledPawn.ActorRotation;
        spawnTransform.Scale = FVector.One;
        APhysicsProjectile projectile = SpawnActor(ProjectileClass, spawnTransform);
        projectile.InitializeFrom(controlledPawn);
        
        FinishExecute(true);
    }
}