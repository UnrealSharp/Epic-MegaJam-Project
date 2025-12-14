using ManagedGameJamProject.AI;
using UnrealSharp;
using UnrealSharp.Attributes;
using UnrealSharp.CoreUObject;
using UnrealSharp.Engine;
using UnrealSharp.GameplayTags;
using UnrealSharp.UnrealSharpCore;

namespace ManagedGameJamProject.Spawn;

[UMultiDelegate]
public delegate void FOnTimeToNextRoundUpdated(int timeRemaining);

[UMultiDelegate]
public delegate void FOnRoundStarted(int roundNumber);

[UClass]
public partial class USpawnSubsystem : UCSWorldSubsystem
{
    [UProperty(PropertyFlags.BlueprintReadOnly)]
    public partial int RoundNumber { get; set; }

    [UProperty(PropertyFlags.BlueprintAssignable)]
    public partial TMulticastDelegate<FOnTimeToNextRoundUpdated> OnTimeToNextRoundUpdated { get; set; }
    
    [UProperty(PropertyFlags.EditAnywhere | PropertyFlags.BlueprintReadOnly)]
    public partial TMulticastDelegate<FOnRoundStarted> OnRoundStarted { get; set; }

    [UProperty] 
    private partial IList<URound> Rounds { get; set; }
    
    public List<AActor> SpawnedEnemies => _spawnedEnemies;
    
    private List<ASpawnerLocation>? _spawnerLocations = null;
    private List<UBKAIBehaviorAsset> _enemyBehaviors = new();
    private readonly List<AActor> _spawnedEnemies = new();
    
    private FTimerHandle _spawnTimer;
    private FTimerHandle _timeToNextRoundTimer;
    private int timeToNextRound;
    
    public override bool DoesSupportWorldType(ECSWorldType worldType)
    {
        return worldType == ECSWorldType.Game || worldType == ECSWorldType.PIE;
    }

    public override void Initialize(FSubsystemCollectionBaseRef collection)
    {
        base.Initialize(collection);
        Initialize();
        
        CreateRound(typeof(UBespokeRound));
        CreateRound(typeof(UBossRound));
        CreateRound(typeof(UMinionsRound));
    }
    
    void CreateRound(TSubclassOf<URound> roundClass)
    {
        URound roundInstance = NewObject(this, roundClass);
        Rounds.Add(roundInstance);
        
        roundInstance.InitializeFrom(this);
    }

    async void Initialize()
    {
        try
        {
            UAssetManager assetManager = UAssetManager.Get();
        
            IList<UBKAIBehaviorAsset> loadedItems = await assetManager.LoadPrimaryAssets<UBKAIBehaviorAsset>(AssetTypes.Behavior.PrimaryAssetList);
            _enemyBehaviors = loadedItems.ToList(); 
        
            UGameplayStatics.GetAllActorsOfClass(typeof(ASpawnerLocation), out IList<ASpawnerLocation> foundActors);
            _spawnerLocations = foundActors.ToList();
        }
        catch (Exception e)
        {
            LogBK.LogError("Error initializing SpawnSubsystem: " + e.Message);
        }
    }
    
    public UBKAIBehaviorAsset? GetRandomEnemyBehaviorByTag(FGameplayTag tag)
    {
        List<UBKAIBehaviorAsset> filteredBehaviors = _enemyBehaviors.Where(behavior => behavior.SlimeTypeTag.MatchesTagExact(tag)).ToList();
        
        if (filteredBehaviors.Count == 0)
        {
            return null;
        }
        
        int randomIndex = MathLibrary.RandomIntegerInRange(0, filteredBehaviors.Count - 1);
        return filteredBehaviors[randomIndex];
    }
    
    public ASpawnerLocation? GetSpawnerLocationByTag(FGameplayTag tag)
    {
        if (_spawnerLocations == null)
        {
            return null;
        }
        
        foreach (ASpawnerLocation spawner in _spawnerLocations)
        {
            if (spawner.SpawnerTag == tag)
            {
                return spawner;
            }
        }

        return null;
    }
    
    public ASpawnerLocation? GetRandomEnemySpawner()
    {
        if (_spawnerLocations == null || _spawnerLocations.Count == 0)
        {
            return null;
        }
        
        int randomIndex = MathLibrary.RandomIntegerInRange(0, _spawnerLocations.Count - 1);
        return _spawnerLocations[randomIndex];
    }
    
    [UFunction(FunctionFlags.BlueprintCallable)]
    public void StartRoundSystem()
    {
        if (_spawnTimer.IsValid())
        {
            return;
        }
        
        _timeToNextRoundTimer = SystemLibrary.SetTimer(TickTimeToNextRound, 1.0f, true);
        timeToNextRound = 10;
    }
    
    [UFunction(FunctionFlags.BlueprintCallable)]
    public ACharacter SpawnEnemy(FVector location, UBKAIBehaviorAsset behaviorAsset, bool manuallyManaged = true, float startingHealth = -1f)
    {
        FTransform spawnTransform = new()
        {
            Scale = FVector.One,
            Location = location
        };

        ABKSlimeCharacter slimeCharacter = SpawnActor(behaviorAsset.SlimeClass, spawnTransform, ESpawnActorCollisionHandlingMethod.AlwaysSpawn);
        slimeCharacter.OverrideBehavior(behaviorAsset, startingHealth);
        
        if (!manuallyManaged)
        {
            _spawnedEnemies.Add(slimeCharacter);
            slimeCharacter.OnEndPlay += OnEnemyEndPlay;
        }

        return slimeCharacter; 
    }
    
    [UFunction]
    void OnEnemyEndPlay(AActor actor, EEndPlayReason endPlayReason)
    {
        if (endPlayReason != EEndPlayReason.Destroyed)
        {
            return;
        }
        
        _spawnedEnemies.Remove(actor);
        actor.OnEndPlay -= OnEnemyEndPlay;

        if (_spawnedEnemies.Count != 0)
        {
            return;
        }
        
        StartRoundSystem();
    }
    
    [UFunction]
    public void TickTimeToNextRound()
    {
        timeToNextRound--;
        OnTimeToNextRoundUpdated.Invoke(timeToNextRound);

        if (timeToNextRound > 0)
        {
            return;
        }
        
        RoundNumber++;
        OnRoundStarted.Invoke(RoundNumber);
        
        bool didStartRound = false;
        foreach (URound round in Rounds)
        {
            if (!round.CanStartNextRound())
            {
                continue;
            }
            
            round.StartRound();
            didStartRound = true;
            break;
        }
        
        if (!didStartRound)
        {
            LogBK.LogWarning("No more rounds available to start.");
            return;
        }
        
        SystemLibrary.ClearAndInvalidateTimerHandle(ref _timeToNextRoundTimer);
    }
}