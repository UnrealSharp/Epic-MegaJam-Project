using UnrealSharp.Attributes;
using UnrealSharp.UnrealSharpCore;

namespace ManagedGameJamProject.AI;

[UStruct]
public partial struct FBossMinionBehavior
{
    [UProperty(PropertyFlags.EditDefaultsOnly | PropertyFlags.BlueprintReadOnly)]
    public UBKAIBehaviorAsset MinionBehavior;

    [UProperty(PropertyFlags.EditDefaultsOnly | PropertyFlags.BlueprintReadOnly)]
    public int MinionCount;
}

[UClass]
public partial class UBossEncounterAsset : UCSPrimaryDataAsset
{
    public UBossEncounterAsset()
    {
        AssetName = "BossEncounter";
    }

    [UProperty(PropertyFlags.EditDefaultsOnly | PropertyFlags.BlueprintReadOnly)]
    public partial UBKAIBehaviorAsset BossBehavior { get; set; }
    
    [UProperty(PropertyFlags.EditDefaultsOnly | PropertyFlags.BlueprintReadOnly)]
    public partial IList<FBossMinionBehavior> MinionBehaviors { get; set; }
}