using UnrealSharp.Attributes;
using UnrealSharp.Engine;

namespace ManagedGameJamProject.Core;

[UClass]
public partial class UBKGameInstance : UGameInstance
{
    [UProperty(PropertyFlags.EditAnywhere | PropertyFlags.BlueprintReadOnly)]
    public partial bool RunTutorial { get; set; }
}