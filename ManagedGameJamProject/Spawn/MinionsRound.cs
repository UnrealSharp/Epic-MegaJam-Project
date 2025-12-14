using ManagedGameJamProject.AI;
using UnrealSharp.Attributes;
using UnrealSharp.Engine;

namespace ManagedGameJamProject.Spawn;

[UClass]
public partial class UMinionsRound : UMinionsRoundBase
{
    private readonly int _baseEnemyCount = 4;

    protected override void SpawnMinionWave()
    {
        USpawnSubsystem spawnSubsystem = GetWorldSubsystem<USpawnSubsystem>();
        UBKAIBehaviorAsset? selectedBehavior = spawnSubsystem.GetRandomEnemyBehaviorByTag(GameplayTags.Game_Enemy_Type_Minion);
        
        if (selectedBehavior == null)
        {
            LogBK.LogWarning("No enemy behavior found with Minion tag.");
            return;
        }
        
        ASpawnerLocation? spawnerLocation = spawnSubsystem.GetRandomEnemySpawner();
            
        if (spawnerLocation == null)
        {
            LogBK.LogWarning("No spawner locations found in the level.");
            return;
        }
        
        double launchMultiplier = MathLibrary.RandomFloatInRange(800.0f, 1500.0f);
        SpawnAndLaunchEnemy(spawnerLocation, selectedBehavior, launchMultiplier);
        
        int enemiesThisRound = _baseEnemyCount * (spawnSubsystem.RoundNumber + 1);
        
        if (spawnSubsystem.SpawnedEnemies.Count >= enemiesThisRound)
        {
            StopSpawning();
        }
    }
}