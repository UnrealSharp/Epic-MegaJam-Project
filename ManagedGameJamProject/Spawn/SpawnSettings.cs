using UnrealSharp;
using UnrealSharp.Attributes;
using UnrealSharp.DeveloperSettings;

namespace ManagedGameJamProject.Spawn;

[UClass(ClassFlags.Config | ClassFlags.DefaultConfig)]
public partial class USpawnSettings : UDeveloperSettings
{
    [UProperty(PropertyFlags.Config | PropertyFlags.EditDefaultsOnly)]
    public partial IList<TSoftObjectPtr<UWaveAsset>> WaveAssets { get; set; }
}