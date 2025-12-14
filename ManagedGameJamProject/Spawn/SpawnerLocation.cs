using UnrealSharp.Attributes;
using UnrealSharp.Engine;
using UnrealSharp.GameplayTags;

namespace ManagedGameJamProject.Spawn;

[UClass]
public partial class ASpawnerLocation : AActor
{
    [UProperty(DefaultComponent = true)]
    public partial USceneComponent Root { get; set; }
    
    [UProperty(DefaultComponent = true, AttachmentComponent = "Root")]
    public partial UArrowComponent Arrow { get; set; }
    
    [UProperty(PropertyFlags.EditAnywhere | PropertyFlags.BlueprintReadOnly)]
    public partial FGameplayTag SpawnerTag { get; set; }
}