using UnrealSharp;
using UnrealSharp.Attributes;
using UnrealSharp.Engine;

namespace ManagedGameJamProject.Equipment;

[UMultiDelegate]
public delegate void FOnEquipmentChanged(UEquipment oldEquipment, UEquipment newEquipment);

[UClass]
public partial class UEquipmentComponent : UActorComponent
{
    [UProperty(PropertyFlags.BlueprintReadOnly)]
    public partial UEquipment? CurrentEquipment { get; set; }
    
    [UProperty(PropertyFlags.BlueprintAssignable)]
    public partial TMulticastDelegate<FOnEquipmentChanged> OnEquipmentChanged { get; set; }

    private float _oldMoveSpeed;

    public override void Tick(float deltaSeconds)
    {
        base.Tick(deltaSeconds);
        
        if (CurrentEquipment != null)
        {
            CurrentEquipment.TickEquipment(deltaSeconds);
        }
    }

    [UFunction(FunctionFlags.BlueprintCallable)]
    public void Equip(TSubclassOf<UEquipment> equipment)
    {
        UEquipment? oldEquipment = CurrentEquipment;
        UnequipCurrent();

        CurrentEquipment = NewObject(Owner, equipment);
        CurrentEquipment.InitializeFromOwner(Owner);
        CurrentEquipment.Equip();
        
        if (Owner is ACharacter character)
        {
            _oldMoveSpeed = character.CharacterMovement.MaxWalkSpeed;
            character.CharacterMovement.MaxWalkSpeed = _oldMoveSpeed * 0.5f;
        }
        
        OnEquipmentChanged.Invoke(oldEquipment!, CurrentEquipment);
    }
    
    [UFunction(FunctionFlags.BlueprintCallable)]
    public void UnequipCurrent()
    {
        if (CurrentEquipment == null)
        {
            return;
        }
        
        UEquipment oldEquipment = CurrentEquipment;
        
        CurrentEquipment.UnEquip();
        CurrentEquipment = null;
        
        if (Owner is ACharacter character)
        {
            character.CharacterMovement.MaxWalkSpeed = 600.0f;
        }
        
        OnEquipmentChanged.Invoke(oldEquipment, null!);
    }
}