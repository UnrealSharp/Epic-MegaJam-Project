using UnrealSharp.Attributes;
using UnrealSharp.Engine;

namespace ManagedGameJamProject.Spawn;

[UClass]
public partial class UMinionsRoundBase : URound
{
    protected FTimerHandle SpawnTimer;
    
    public override void StartRound()
    {
        base.StartRound();
        SpawnTimer = SystemLibrary.SetTimer(SpawnMinionWave, 0.9f, true);
    }

    [UFunction]
    protected virtual void SpawnMinionWave()
    {
        
    }
    
    protected void StopSpawning()
    {
        SystemLibrary.ClearAndInvalidateTimerHandle(ref SpawnTimer);
    }
}