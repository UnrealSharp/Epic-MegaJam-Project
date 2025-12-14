using ManagedGameJamProject.Components;
using UnrealSharp.Attributes;
using UnrealSharp.Engine;
using UnrealSharp.GameplayTags;

namespace ManagedGameJamProject.Item;

[UClass]
public partial class UStatsApplier : UConsumable
{
    [UProperty(PropertyFlags.EditAnywhere | PropertyFlags.BlueprintReadOnly)]
    public partial IDictionary<FGameplayTag, float> StatsToApply { get; set; }

    public override void ConsumeItem(APawn pawn)
    {
        UStatsComponent attributeComponent = pawn.GetComponentByClass<UStatsComponent>();
        foreach (KeyValuePair<FGameplayTag, float> stat in StatsToApply)
        {
            attributeComponent.ModifyStat(stat.Key, stat.Value);
        }
    }
}