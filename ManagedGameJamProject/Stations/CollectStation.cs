using ManagedGameJamProject.Components;
using ManagedGameJamProject.Interaction;
using ManagedGameJamProject.Subsystems;
using UnrealSharp;
using UnrealSharp.Attributes;
using UnrealSharp.Engine;

namespace ManagedGameJamProject.Stations;

[UClass]
public partial class UTransferSlime : UInteraction
{
    public readonly float TransferRatePerSecond = 25f;
    
    private UBKSlimeInventory ownerInventory;
    private UBKSlimeInventory interactorInventory;
    private FTimerHandle transferTimerHandle;

    protected override void OnInitialize()
    { 
        UBKSlimeInventory inventory = OwningInteractionComponent.Owner.GetComponentByClass<UBKSlimeInventory>();
        
        if (inventory == null)
        {
            LogBK.LogWarning("TransferSlime Interaction: No Slime Inventory found on Owner Actor.");
            return;
        }
        
        ownerInventory = inventory;
    }

    public override void OnInteract(AActor interactor)
    {
        UBKSlimeInventory inventory = interactor.GetComponentByClass<UBKSlimeInventory>();
        if (inventory == null)
        {
            LogBK.LogWarning("TransferSlime Interaction: No Slime Inventory found on Interacting Actor.");
            return;
        }
        
        interactorInventory = inventory;
        
        UCharacterMovementComponent movementComponent = interactor.GetComponentByClass<UCharacterMovementComponent>();
        if (movementComponent == null)
        {
            LogBK.LogWarning("TransferSlime Interaction: No Movement Component found on Interacting Actor.");
            return;
        }
        
        movementComponent.DisableMovement();
        movementComponent.StopMovementImmediately();
        
        transferTimerHandle = SystemLibrary.SetTimer(this, nameof(TransferSlimeAmount), 0.1f, true);
    }
    
    [UFunction]
    public void TransferSlimeAmount()
    {
        float amountToTransfer = TransferRatePerSecond * UGameplayStatics.WorldDeltaSeconds.ToFloat();
        float actualTransferred = interactorInventory.AddSlime(-amountToTransfer);
        ownerInventory.AddSlime(actualTransferred);
    }

    public override void OnEndInteract(AActor interactor)
    {
        SystemLibrary.ClearAndInvalidateTimerHandle(ref transferTimerHandle);
        
        UCharacterMovementComponent movementComponent = interactor.GetComponentByClass<UCharacterMovementComponent>();
        
        if (movementComponent == null)
        {
            LogBK.LogWarning("TransferSlime Interaction: No Movement Component found on Interacting Actor.");
            return;
        }
        
        movementComponent.SetMovementMode(EMovementMode.MOVE_Walking);
    }
}

[UClass]
public partial class ACollectStation : AActor
{
    [UProperty(DefaultComponent = true, RootComponent = true)]
    public partial USceneComponent Root { get; set; }
    
    [UProperty(DefaultComponent = true, AttachmentComponent = "Root")]
    public partial UStaticMeshComponent Mesh { get; set; }
    
    [UProperty(DefaultComponent = true)]
    public partial UBKSlimeInventory SlimeInventory { get; set; }
    
    [UProperty(DefaultComponent = true)]
    public partial USphereComponent ProximitySphere { get; set; }
    
    [UProperty]
    public partial UBKSlimeLevelManager SlimeLevelManager { get; set; }

    public override void BeginPlay()
    {
        base.BeginPlay();
        SlimeLevelManager = NewObject<UBKSlimeLevelManager>(this);
        SlimeLevelManager.InitializeFrom(SlimeInventory, Mesh);

        UStoredGooManager storedGooManager = GetWorldSubsystem<UStoredGooManager>();
        storedGooManager.RegisterGooStorageInventory(SlimeInventory);
    }
}