using ManagedGameJamProject.Core;
using UnrealSharp.Attributes;
using UnrealSharp.Engine;

namespace ManagedGameJamProject.Spawn;

[UClass]
public partial class ASpawnTriggerVolume : AActor
{
    [UProperty(DefaultComponent = true, RootComponent = true)]
    public partial UBoxComponent TriggerVolume { get; set; }

    public override void BeginPlay()
    {
        base.BeginPlay();

        UBKGameInstance gameInstance = World.GameInstanceAs<UBKGameInstance>();
        if (!gameInstance.RunTutorial)
        {
            USpawnSubsystem spawnSubsystem = GetWorldSubsystem<USpawnSubsystem>();
            spawnSubsystem.StartRoundSystem();
        }
    }

    public override void ActorBeginOverlap(AActor otherActor)
    {
        base.ActorBeginOverlap(otherActor);

        if (otherActor is not ABKCharacter)
        {
            return;
        }

        USpawnSubsystem spawnSubsystem = GetWorldSubsystem<USpawnSubsystem>();
        spawnSubsystem.StartRoundSystem();
        
        UBKGameInstance gameInstance = World.GameInstanceAs<UBKGameInstance>();
        gameInstance.RunTutorial = false;
    }
}