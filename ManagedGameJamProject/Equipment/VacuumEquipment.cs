using ManagedGameJamProject.Components;
using ManagedGameJamProject.Resources;
using ManagedGameJamProject.Stations;
using ManagedGameJamProject.Utilities;
using UnrealSharp;
using UnrealSharp.Attributes;
using UnrealSharp.CoreUObject;
using UnrealSharp.Engine;

namespace ManagedGameJamProject.Equipment;

[UClass]
public partial class UVacuumEquipment : UEquipment
{
    [UProperty(PropertyFlags.EditDefaultsOnly | PropertyFlags.BlueprintReadOnly)]
    public partial float VacuumRange { get; set; }
    
    [UProperty(PropertyFlags.EditDefaultsOnly | PropertyFlags.BlueprintReadOnly)]
    public partial float TransferRate { get; set; }
    
    private UBKSlimeInventory _slimeInventory = null!;
    private UBKSlimeInventory? _collectorInventory;
    private FTimerHandle _transferTimerHandle;
    private UAudioComponent _component;

    public UVacuumEquipment()
    {
        IntervalBetweenAttacks = 0.2f;
        SpeedStatTag = GameplayTags.Game_Stats_VacuumSpeed;
    }

    public override void InitializeFromOwner(AActor character)
    {
        base.InitializeFromOwner(character);
        _slimeInventory = Owner!.GetComponentByClass<UBKSlimeInventory>();
    }

    protected override void OnUse()
    {
        FVector start = Owner!.ActorLocation;
        List<AActor> ignoredActors = new List<AActor> { Owner! };
        if (!SystemLibrary.MultiSphereTraceByChannel(start, start, VacuumRange, ETraceChannel.Visibility.ToTraceQuery(),
                false,
                ignoredActors,
                EDrawDebugTrace.None,
                out IList<FHitResult>? hitResults, true))
        {
            return;
        }

        ACharacter ownerCharacter = (ACharacter)Owner!;
        
        void OnVanishComplete(UBKSlimeInventory inventory)
        {
            _slimeInventory.AddSlime(inventory.CurrentValue);
        }
        
        FVanishParameters vanishParameters = new FVanishParameters(OnVanishComplete, ownerCharacter.Mesh, BKUtilities.BoltSocket);
            
        foreach (FHitResult hitResult in hitResults)
        {
            UPrimitiveComponent? hitComponent = hitResult.Component.Object;
            if (hitComponent == null)
            {
                continue;
            }
            
            if (!_slimeInventory.IsFull() && hitComponent.Owner is ASlimeResource resource)
            {
                resource.StartVanish(vanishParameters);
            }
            else if (hitComponent.Owner is ACollectStation collectStation)
            {
                TryStartTransfer(collectStation);
            }
        }
    }

    protected override void OnEquip()
    {
        if (_component.IsValid())
        {
            _component.Play();
        }
        else
        {
            UAudioComponent component = UGameplayStatics.SpawnSoundAttached(EquipmentSound, Owner!.RootComponent);
            _component = component;
        }
        
        base.OnEquip();
    }

    protected override void OnUnEquip()
    {
        base.OnUnEquip();
        StopTransfer();
        
        if (_component.IsValid())
        {
            _component.Stop();
        }
    }

    void TryStartTransfer(ACollectStation collectStation)
    {
        if (_slimeInventory.CurrentValue <= 0 || _transferTimerHandle.IsValid())
        {
            return;
        }
        
        _collectorInventory = collectStation.GetComponentByClass<UBKSlimeInventory>();
        
        if (_collectorInventory == null)
        {
            LogBK.LogWarning("Collect station has no slime inventory!");
            return;
        }
        
        _transferTimerHandle = SystemLibrary.SetTimer(TransferSlimeToStation, 0.1f, true);
    }
    
    [UFunction]
    void TransferSlimeToStation()
    {
        float amountToTransfer = TransferRate * UGameplayStatics.WorldDeltaSeconds.ToFloat();
        float actualTransferred = _slimeInventory.AddSlime(-amountToTransfer);
        _collectorInventory!.AddSlime(actualTransferred);
        
        if (_slimeInventory.CurrentValue <= 0)
        {
            StopTransfer();
        }
    }
    
    void StopTransfer()
    {
        if (!_transferTimerHandle.IsValid())
        {
            return;
        }
        
        SystemLibrary.ClearAndInvalidateTimerHandle(ref _transferTimerHandle);
        _collectorInventory = null;
    }
}