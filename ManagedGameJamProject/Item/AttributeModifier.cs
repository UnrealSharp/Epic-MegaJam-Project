using ManagedGameJamProject.Components;
using UnrealSharp;
using UnrealSharp.Attributes;
using UnrealSharp.Core.Attributes;
using UnrealSharp.Engine;

namespace ManagedGameJamProject.Item;

[UEnum]
public enum EModifierType : byte
{
    Additive = 0,
    Multiplicative = 1,
}

[UStruct]
public partial struct FAttributeModifierData
{
    [UProperty]
    public bool HasModifier;
    
    [UProperty(PropertyFlags.EditAnywhere | PropertyFlags.BlueprintReadOnly), UMetaData("EditCondition", "HasModifier")]
    public float Value;

    [UProperty(PropertyFlags.EditAnywhere | PropertyFlags.BlueprintReadOnly), UMetaData("EditCondition", "HasModifier")]
    public EModifierType ModifierType;
}

[UClass]
public partial class UAttributeModifier : UConsumable
{
    [UProperty(PropertyFlags.EditAnywhere)]
    public partial TSubclassOf<UAttributeComponent> AttributeComponentClass { get; set; }
    
    [UProperty(PropertyFlags.EditAnywhere | PropertyFlags.BlueprintReadOnly), UMetaData("EditCondition", "HasMin")]
    public partial FAttributeModifierData CurrentValue { get; set; }
    
    [UProperty(PropertyFlags.EditAnywhere | PropertyFlags.BlueprintReadOnly), UMetaData("EditCondition", "HasMax")]
    public partial FAttributeModifierData MaxValue { get; set; }

    public override void ConsumeItem(APawn pawn)
    {
        UAttributeComponent attributeComponent = pawn.GetComponentByClass(AttributeComponentClass);

        if (attributeComponent == null)
        {
            return;
        }
        
        if (CurrentValue.HasModifier)
        {
            float newCurrent = CurrentValue.ModifierType == EModifierType.Additive
                ? attributeComponent.CurrentValue + CurrentValue.Value
                : attributeComponent.CurrentValue * CurrentValue.Value;
                
            attributeComponent.SetCurrentValue(newCurrent);
        }

        if (MaxValue.HasModifier)
        {
            float newMax = MaxValue.ModifierType == EModifierType.Additive
                ? attributeComponent.MaxValue + MaxValue.Value
                : attributeComponent.MaxValue * MaxValue.Value;
                
            attributeComponent.SetMaxValue(newMax);
        }
    }
}