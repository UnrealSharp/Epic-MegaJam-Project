using ManagedGameJamProject.Utilities;
using UnrealSharp;
using UnrealSharp.Attributes;
using UnrealSharp.CoreUObject;
using UnrealSharp.Engine;
using UnrealSharp.UnrealSharpCore;

namespace ManagedGameJamProject.Equipment;

[UClass]
public partial class AArrow : AActor
{
    [UProperty(PropertyFlags.BlueprintReadOnly)]
    protected partial float InitialDamage { get; set; }
    
    public void Initialize(float inDamage)
    {
        InitialDamage = inDamage;
    }
    
    [UProperty(DefaultComponent = true, RootComponent = true)]
    public partial UStaticMeshComponent Mesh { get; set; }
    
    [UProperty(DefaultComponent = true)]
    public partial UProjectileMovementComponent ProjectileMovement { get; set; }
    
    [UFunction(FunctionFlags.BlueprintEvent)]
    protected partial float DetermineDamage(AActor hitActor);
    protected partial float DetermineDamage_Implementation(AActor hitActor)
    {
        return InitialDamage;
    }

    public override void ActorBeginOverlap(AActor otherActor)
    {
        if (otherActor is not APawn || Equals(otherActor, Owner))
        {
            return;
        }
        
        UBKHealthComponent healthComponent = otherActor.GetComponentByClass<UBKHealthComponent>();
        if (healthComponent == null || healthComponent.IsDead)
        {
            return;
        }
        
        float finalDamage = DetermineDamage(otherActor);
        healthComponent.ApplyDamage(finalDamage);
        DestroyActor();
    }
}

[UClass]
public partial class UBowEquipment : UEquipment
{
    [UProperty(PropertyFlags.EditDefaultsOnly)]
    public partial TSubclassOf<AArrow> ArrowClass { get; set; }
    
    [UProperty(PropertyFlags.EditDefaultsOnly)]
    public partial float Damage { get; set; }
    
    private FRotator _currentRotation;
    private FRotator _targetRotation;
    private bool _hasShot = false;
    
    private ACharacter _ownerCharacter;
    private FRotator _initialRotation;
    
    public UBowEquipment()
    {
        IntervalBetweenAttacks = 0.2f;
        SpeedStatTag = GameplayTags.Game_Stats_ShootSpeed;
    }

    protected override void OnEquip()
    {
        base.OnEquip();
        _ownerCharacter = (ACharacter)Owner!;
        _initialRotation = _ownerCharacter.Mesh.RelativeRotation;
    }

    protected override void OnUnEquip()
    {
        base.OnUnEquip();
        _ownerCharacter.Mesh.SetRelativeRotation(_initialRotation, false, out _, false);
    }

    protected override void OnUse()
    {
        base.OnUse();
        
        UGameplayStatics.PlaySoundAtLocation(EquipmentSound, Owner!.ActorLocation, FRotator.ZeroRotator);

        FTransform spawnTransform = Owner!.ActorTransform;
        APlayerController localController = UGameplayStatics.GetPlayerController(0);
        localController.ConvertMouseLocationToWorldSpace(out FVector worldLocation, out FVector worldDirection);
        
        MathLibrary.LinePlaneIntersection_OriginNormal(worldLocation, worldDirection * 200000.0f, Owner.ActorLocation, FVector.Up, out _, out FVector intersection);
        
        FVector socketLocation = _ownerCharacter.Mesh.GetSocketLocation(BKUtilities.BoltSocket);
        FVector direction = FVector.Normalize(intersection - Owner.ActorLocation);
        
        FRotator rotation = MathLibrary.MakeRotFromX(direction);
        rotation.Roll = 0.0f;
        rotation.Pitch = 0.0f;
        
        spawnTransform.Rotation = rotation;
        spawnTransform.Location = socketLocation; 
        
        _targetRotation = rotation;
        _targetRotation.Yaw -= 90.0f;
        
        FCSSpawnActorParameters spawnParams = new();
        spawnParams.Owner = Owner;
        
        SpawnActorDeferred(spawnTransform, ArrowClass, spawnParams, null, arrow =>
        {
            arrow.Initialize(Damage);   
        });

        _hasShot = true;
    }

    public override void TickEquipment(float deltaTime)
    {
        if (!_hasShot)
        {
            return;
        }
        
        _currentRotation = MathLibrary.RInterpTo(_currentRotation, _targetRotation, deltaTime, 40.0f);
        _ownerCharacter.Mesh.SetWorldRotation(_currentRotation, false, out _, false);
    }
    
}