using UnrealSharp.Attributes;
using UnrealSharp.Core;
using UnrealSharp.Engine;

namespace ManagedGameJamProject.Core;

[UClass]
public partial class ABKGameMode : AGameModeBase
{
    APlayerStart? FindPlayerStartByTag(FName tag)
    {
        UGameplayStatics.GetAllActorsOfClass(typeof(APlayerStart), out IList<AActor> actors);
        
        foreach (AActor actor in actors)
        {
            APlayerStart playerStart = (APlayerStart)actor;
            
            if (playerStart.PlayerStartTag == tag)
            {
                return playerStart;
            }
            
        }

        return null;
    }
    
    public override AActor ChoosePlayerStart(AController player)
    {
        UBKGameInstance gameInstance = World.GameInstanceAs<UBKGameInstance>();
        
        APlayerStart? playerStart;
        if (gameInstance.RunTutorial)
        {
            playerStart = FindPlayerStartByTag("Tutorial");
        }
        else
        {
            playerStart = FindPlayerStartByTag("Arena");
        }
        
        if (playerStart != null)
        {
            return playerStart;
        }
        
        return base.ChoosePlayerStart(player);
    }
}