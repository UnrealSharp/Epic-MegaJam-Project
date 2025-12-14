using ManagedGameJamProject.AI.Tasks;
using UnrealSharp.Attributes;
using UnrealSharp.UnrealSharpCore;

namespace ManagedGameJamProject.Spawn;

[UClass]
public partial class UWaveAsset : UCSPrimaryDataAsset
{
    public UWaveAsset()
    {
        AssetName = "WaveAsset";
    }
    
    [UProperty(PropertyFlags.EditDefaultsOnly)]
    public partial IList<FMinionInfo> MinionsToSpawn { get; set; }
}