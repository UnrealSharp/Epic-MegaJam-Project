using ManagedGameJamProject.Spawn;
using UnrealSharp.AIModule;
using UnrealSharp.Attributes;
using UnrealSharp.Core.Attributes;
using UnrealSharp.CoreUObject;
using UnrealSharp.Engine;

namespace ManagedGameJamProject.AI.Tasks;

[UStruct]
public partial struct FMinionInfo
{
    [UProperty(PropertyFlags.EditAnywhere | PropertyFlags.BlueprintReadWrite)]
    public UBKAIBehaviorAsset BehaviorAsset { get; set; }
    
    [UProperty(PropertyFlags.EditAnywhere | PropertyFlags.BlueprintReadWrite)]
    public int MinionCount { get; set; }
    
    [UProperty]
    public bool OverrideStartingHealth { get; set; }
    
    [UProperty(PropertyFlags.EditAnywhere | PropertyFlags.BlueprintReadWrite), UMetaData("EditCondition", "OverrideStartingHealth")]
    public float StartingHealth { get; set; }
    
    public float GetStartingHealthOrDefault()
    {
        return OverrideStartingHealth ? StartingHealth : -1;
    }
}

[UEnum]
public enum EMinionSpawnPattern : byte
{
    Circle,
    Line
}

[UClass]
public partial class USpawnMinionsTask : UBTTask_BlueprintBase
{
    [UProperty(PropertyFlags.EditAnywhere)]
    public partial IList<FMinionInfo> MinionsToSpawn { get; set; }
    
    [UProperty(PropertyFlags.EditAnywhere)]
    public partial float SpawnRadius { get; set; }
    
    [UProperty(PropertyFlags.EditAnywhere)]
    public partial EMinionSpawnPattern SpawnPattern { get; set; }
    
    public override void ReceiveExecuteAI(AAIController ownerController, APawn controlledPawn)
    {
        USpawnSubsystem spawnSubsystem = GetWorldSubsystem<USpawnSubsystem>();
        
        int totalMinions = 0;
        foreach (FMinionInfo minionInfo in MinionsToSpawn)
        {
            totalMinions += minionInfo.MinionCount;
        }
        
        int currentMinionIndex = 0;
        if (SpawnPattern == EMinionSpawnPattern.Circle)
        {
            foreach (FMinionInfo minionInfo in MinionsToSpawn)
            {
                for (int i = 0; i < minionInfo.MinionCount; i++)
                {
                    double angle = (currentMinionIndex / (float)totalMinions) * 2.0f * MathLibrary.PI;
                    FVector spawnLocation = controlledPawn.ActorLocation + new FVector(MathLibrary.Cos(angle) * SpawnRadius, MathLibrary.Sin(angle) * SpawnRadius, 0);
                    spawnSubsystem.SpawnEnemy(spawnLocation, minionInfo.BehaviorAsset, false, minionInfo.GetStartingHealthOrDefault());
                    currentMinionIndex++;
                }
            }
        }
        else
        {
            void SpawnAlongLine(FVector direction, int spawnPoints)
            {
                foreach (FMinionInfo minionInfo in MinionsToSpawn)
                {
                    for (int i = 0; i < minionInfo.MinionCount; i++)
                    {
                        FVector spawnLocation = controlledPawn.ActorLocation + direction * (i + 1) / spawnPoints;
                        spawnSubsystem.SpawnEnemy(spawnLocation, minionInfo.BehaviorAsset, false, minionInfo.GetStartingHealthOrDefault());
                    }
                }
            }
            
            FVector forward = ActorOwner.ActorForwardVector * SpawnRadius;
            FVector behind = -forward;
            FVector right = ActorOwner.ActorRightVector * SpawnRadius;
            FVector left = -right;
            
            int spawnPoints = (int) SpawnRadius / 100;
            SpawnAlongLine(forward, spawnPoints);
            SpawnAlongLine(behind, spawnPoints);
            SpawnAlongLine(right, spawnPoints);
            SpawnAlongLine(left, spawnPoints);
        }
        
        FinishExecute(true);
    }
}