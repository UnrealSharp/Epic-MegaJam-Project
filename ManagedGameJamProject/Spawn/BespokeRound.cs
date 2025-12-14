using ManagedGameJamProject.AI.Tasks;
using UnrealSharp.Attributes;
using UnrealSharp.Engine;

namespace ManagedGameJamProject.Spawn;

[UClass]
public partial class UBespokeRound : UMinionsRoundBase
{
    [UProperty] 
    partial IList<UWaveAsset> WaveAssets { get; set; }
    
    private int _currentWaveIndex;
    
    private int _currentMinionTypeIndex;
    private int _currentMinionCount;
    
    public override void InitializeFrom(USpawnSubsystem subsystem)
    {
        base.InitializeFrom(subsystem);
        LoadWaveAssets();
    }

    async void LoadWaveAssets()
    {
        try
        {
            UAssetManager assetManager = UAssetManager.Get();
            await assetManager.LoadPrimaryAssets<UWaveAsset>(AssetTypes.WaveAsset.PrimaryAssetList);

            USpawnSettings spawnSettings = GetDefault<USpawnSettings>();
            for (int i = 0; i < spawnSettings.WaveAssets.Count; i++)
            {
                UWaveAsset? waveAsset = spawnSettings.WaveAssets[i].Object;
                
                if (waveAsset != null)
                {
                    WaveAssets.Add(waveAsset);
                }
            }
        }
        catch (Exception exception)
        {
            LogBK.LogError("Failed to load wave assets for BespokeRound: " + exception.Message);
        }
    }

    public override bool CanStartNextRound()
    {
        if (WaveAssets.Count == 0)
        {
            LogBK.LogWarning("No wave assets loaded for BespokeRound.");
            return false;
        }
        
        return _currentWaveIndex < WaveAssets.Count;
    }

    protected override void OnEndRound()
    {
        _currentWaveIndex++;
    }

    protected override void SpawnMinionWave()
    {
        USpawnSubsystem spawnSubsystem = GetWorldSubsystem<USpawnSubsystem>();
        ASpawnerLocation? spawnerLocation = spawnSubsystem.GetRandomEnemySpawner();
            
        if (spawnerLocation == null)
        {
            LogBK.LogWarning("No spawner locations found in the level.");
            return;
        }
        
        UWaveAsset waveAsset = WaveAssets[_currentWaveIndex];
        FMinionInfo minionType = waveAsset.MinionsToSpawn[_currentMinionTypeIndex];
        
        double launchMultiplier = MathLibrary.RandomFloatInRange(800.0f, 1500.0f);
        SpawnAndLaunchEnemy(spawnerLocation, minionType.BehaviorAsset, launchMultiplier);
        _currentMinionCount++;
        
        if (_currentMinionCount >= minionType.MinionCount)
        {
            _currentMinionTypeIndex++;
            _currentMinionCount = 0;
        }
        
        if (_currentMinionTypeIndex >= waveAsset.MinionsToSpawn.Count)
        {
            _currentWaveIndex++;
            _currentMinionTypeIndex = 0;
            _currentMinionCount = 0;
            
            StopSpawning();
        }
    }
}