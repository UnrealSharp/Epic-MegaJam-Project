using ManagedGameJamProject.AI;
using UnrealSharp.Attributes;
using UnrealSharp.CoreUObject;
using UnrealSharp.Engine;

namespace ManagedGameJamProject.Spawn;

[UClass]
public partial class URound : UObject
{
    public virtual void InitializeFrom(USpawnSubsystem subsystem)
    {
        
    }
    
    public virtual void StartRound()
    {
        
    }
    
    protected virtual void OnEndRound()
    {
        
    }
    
    public virtual bool CanStartNextRound()
    {
        return true;
    }
    
    protected ACharacter SpawnEnemy(FVector location, UBKAIBehaviorAsset behaviorAsset)
    {
        USpawnSubsystem spawnSubsystem = GetTypedOuter<USpawnSubsystem>();
        return spawnSubsystem.SpawnEnemy(location, behaviorAsset, false);
    }
    
    protected ACharacter SpawnAndLaunchEnemy(ASpawnerLocation spawnerLocation, UBKAIBehaviorAsset behaviorAsset, double launchMultiplier)
    {
        ACharacter spawnedEnemy = SpawnEnemy(spawnerLocation.ActorLocation, behaviorAsset);
        spawnedEnemy.LaunchCharacter(spawnerLocation.Arrow.ForwardVector * launchMultiplier, false, true);
        return spawnedEnemy;
    }
}