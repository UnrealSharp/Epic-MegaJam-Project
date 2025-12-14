using ManagedGameJamProject.Components;
using ManagedGameJamProject.Core;
using ManagedGameJamProject.Resources;
using UnrealSharp;
using UnrealSharp.AIModule;
using UnrealSharp.Attributes;
using UnrealSharp.Core;
using UnrealSharp.CoreUObject;
using UnrealSharp.Engine;

namespace ManagedGameJamProject.AI;

[UClass]
public partial class ABKSlimeCharacter : ABKCharacterBase
{
    [UProperty(DefaultComponent = true)]
    public partial USlimeMeshComponent SlimeMesh { get; set; }
    
    [UProperty(DefaultComponent = true)]
    public partial USphereComponent AttackDetectionSphere { get; set; }
    
    [UProperty(DefaultComponent = true)]
    public partial UBKGrowComponent GrowComponent { get; set; }
    
    [UProperty(DefaultComponent = true)]
    public partial UTimelineComponent DeathTimeline { get; set; }
    
    [UProperty(PropertyFlags.EditDefaultsOnly)]
    public partial TSubclassOf<ASlimeResource> SlimeDropClass { get; set; }
    
    [UProperty(PropertyFlags.BlueprintReadOnly)]
    public partial UBKAIBehaviorAsset CurrentBehavior { get; set; }
    
    [UProperty]
    private partial UBlackboardComponent BlackboardComponent { get; set; }
    
    [UProperty(PropertyFlags.EditAnywhere)]
    public partial UCurveFloat DeathCurve { get; set; }

    public override void ActorBeginOverlap(AActor otherActor)
    {
        if (otherActor is not ASlimeResource)
        {
            return;
        }
        
        ASlimeResource resource = (ASlimeResource) otherActor;
        
        if (ReferenceEquals(resource.Owner, this))
        {
            return;
        }

        void OnVanishComplete(UBKSlimeInventory inventory)
        {
            if (HealthComponent == null || inventory == null)
            {
                return;
            }
            
            HealthComponent.SetCurrentValue(HealthComponent.CurrentValue + inventory.CurrentValue);
        }

        resource.StartVanish(new FVanishParameters(OnVanishComplete, SlimeMesh));
    }

    public void OverrideBehavior(UBKAIBehaviorAsset behavior, float overrideHealth = -1.0f)
    {
        CurrentBehavior = behavior;
        
        ABKAIController? controller = (ABKAIController) Controller;
        if (controller)
        {
            controller.RunBehaviorTree(behavior.BehaviorTree);
            BlackboardComponent = AIHelperLibrary.GetBlackboard(controller);
            BlackboardComponent.SetValueAsObject("TargetActor", UGameplayStatics.GetPlayerCharacter(0));
        }
        
        HealthComponent.OnAttributeChanged += OnHealthChanged;
        
        CharacterMovement.MaxWalkSpeed = behavior.MoveSpeed;
        SlimeMesh.SetStaticMesh(behavior.SlimeMesh);
        HurtComponent.InitializeFrom(SlimeMesh);
        
        HealthComponent.SetMaxValue(100000.0f);

        float startingHealth;
        if (overrideHealth > 0.0f)
        {
            startingHealth = overrideHealth;
        }
        else
        {
            startingHealth = MathLibrary.RandomFloatInRange(behavior.MinStartSlimeAmount, behavior.MaxStartSlimeAmount).ToFloat();
        }

        HealthComponent.SetCurrentValue(startingHealth);
        
        SlimeMesh.ApplySlimeMaterial();
    }

    public override void OnDeath(UBKHealthComponent healthComp)
    {
        base.OnDeath(healthComp);

        if (!this.IsValid())
        {
            return;
        }
        
        ABKAIController? controller = (ABKAIController) Controller;
        
        if (controller == null || !controller.IsValid())
        {
            DestroyActor();
            return;
        }
        
        controller.BrainComponent.StopLogic("Slime Died");
        controller.RunBehaviorTree(null);
        
        TDelegate<FOnTimelineFloat> timelineDelegate = new TDelegate<FOnTimelineFloat>();
        timelineDelegate.BindUFunction(this, "HandleDeathTimelineProgress");
        
        DeathTimeline.AddInterpFloat(DeathCurve, timelineDelegate);
        DeathTimeline.PlayFromStart();
        
        CapsuleComponent.SetCollisionResponseToChannel(UnrealSharp.Engine.ECollisionChannel.ECC_Pawn, ECollisionResponse.ECR_Ignore);
    }
    
    [UFunction]
    public void HandleDeathTimelineProgress(float value)
    {
        SlimeMesh.SetScalarParameterValueOnMaterials("OnDeath", value);
        
        if (value >= 1.0f)
        {
            DestroyActor();
        }
    }
    
    [UFunction()]
    void OnHealthChanged(UAttributeComponent attributeComp, float newHealth, float oldValue)
    {
        if (newHealth < oldValue)
        {
            int damageTaken = (int) (oldValue - newHealth);
            
            bool shootLongerRanger = CurrentBehavior.ChanceToSpawnLongDistanceSlime >= MathLibrary.RandomFloatInRange(0.0f, 1.0f);
            FVector spawnLocation = ActorLocation;
            FRotator spawnRotation = FRotator.ZeroRotator;
            
            if (shootLongerRanger)
            {
                SpawnGroupOfSlimes(damageTaken, spawnLocation, spawnRotation, 400.0f, 200.0f);
            }
            else
            {
                SpawnGroupOfSlimes(damageTaken, spawnLocation, spawnRotation, 500.0f, 700.0f);
            }
        }
    }

    void SpawnGroupOfSlimes(int amount, FVector spawnLocation, FRotator spawnRotation, float upwardForce, float outwardForce)
    {
        for (int i = 0; i < amount; i++)
        {
            double angle = MathLibrary.RandomFloatInRange(0.0f, 360.0f);
            FVector launchVelocity = FVector.Up * upwardForce; 
            launchVelocity += new FVector(MathLibrary.DegCos(angle), MathLibrary.DegSin(angle), 0.0f) * outwardForce;
            
            FTransform spawnTransform = new FTransform();
            spawnTransform.Location = spawnLocation;
            spawnTransform.Rotation = spawnRotation;
            spawnTransform.Scale = FVector.One;
            
            ASlimeResource slime = SpawnActor(SlimeDropClass, spawnTransform, ESpawnActorCollisionHandlingMethod.AlwaysSpawn, null, this);
            CurrentBehavior.ApplyRandomMesh(slime.Mesh);
            slime.Mesh.ApplySlimeMaterial();
            slime.Mesh.SimulatePhysics = true;
            slime.Mesh.AddImpulse(launchVelocity, FName.None, true);
        }
    }
}