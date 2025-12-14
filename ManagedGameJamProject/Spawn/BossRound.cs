using ManagedGameJamProject.AI;
using UnrealSharp.Attributes;

namespace ManagedGameJamProject.Spawn;

[UClass]
public partial class UBossRound : URound
{
    public override bool CanStartNextRound()
    {
        USpawnSubsystem spawnSubsystem = GetWorldSubsystem<USpawnSubsystem>();
        return spawnSubsystem.RoundNumber % 3 == 0;
    }

    public override void StartRound()
    {
        USpawnSubsystem spawnSubsystem = GetWorldSubsystem<USpawnSubsystem>();
        UBKAIBehaviorAsset? bossBehavior = spawnSubsystem.GetRandomEnemyBehaviorByTag(GameplayTags.Game_Enemy_Type_Boss);
        
        if (bossBehavior == null)
        {
            LogBK.LogWarning("No boss behavior found with Boss tag.");
            return;
        }
        
        ASpawnerLocation? bossSpawner = spawnSubsystem.GetSpawnerLocationByTag(GameplayTags.Game_Enemy_Type_Boss);
        if (bossSpawner == null)
        {
            LogBK.LogWarning("No boss spawner location found in the level.");
            return;
        }
        
        SpawnEnemy(bossSpawner.ActorLocation, bossBehavior);
    }
}