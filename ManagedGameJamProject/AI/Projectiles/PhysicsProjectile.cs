using UnrealSharp;
using UnrealSharp.Attributes;
using UnrealSharp.Core;
using UnrealSharp.CoreUObject;
using UnrealSharp.Engine;

namespace ManagedGameJamProject.AI.Projectiles;

[UClass]
public partial class APhysicsProjectile : AActor
{
    [UProperty(DefaultComponent = true, RootComponent = true)]
    public partial UStaticMeshComponent ProjectileMesh { get; set; }
    
    [UProperty(PropertyFlags.EditDefaultsOnly)]
    public partial TSubclassOf<AHitEffectActor> HitEffectClass { get; set; }

    public void InitializeFrom(APawn controlledPawn)
    {
        ProjectileMesh.SimulatePhysics = true;
        ACharacter character = UGameplayStatics.GetPlayerCharacter(0);
        FVector potentialFutureLocation = character.ActorLocation + character.Velocity * 1.0f;
        UGameplayStatics.SuggestProjectileVelocityCustomArc(out FVector outLaunchVelocity, controlledPawn.ActorLocation,
            potentialFutureLocation);
        
        ProjectileMesh.AddImpulse(outLaunchVelocity, FName.None, true);
    }

    public override void Hit(UPrimitiveComponent myComp, AActor other, UPrimitiveComponent otherComp,
        bool selfMoved,
        FVector hitLocation, FVector hitNormal, FVector normalImpulse, FHitResult hit)
    {
        FTransform spawnTransform = new FTransform();
        spawnTransform.Location = hitLocation;
        spawnTransform.Rotation = FRotator.ZeroRotator;
        spawnTransform.Scale = FVector.One;
            
        AHitEffectActor effectActor = SpawnActor(HitEffectClass, spawnTransform);
        effectActor.Initialize();
        
        DestroyActor();
    }
}