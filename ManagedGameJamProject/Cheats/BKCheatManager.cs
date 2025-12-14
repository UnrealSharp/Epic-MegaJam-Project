using ManagedGameJamProject.AI;
using ManagedGameJamProject.Core;
using UnrealSharp.Attributes;
using UnrealSharp.Engine;

namespace ManagedGameJamProject.Cheats;

[UClass]
public partial class UBKCheatManager : UCheatManager
{
    [UFunction(FunctionFlags.BlueprintCallable | FunctionFlags.Exec)]
    public void GiveSlime(float amount)
    {
        OwningCharacter.SlimeInventory.AddSlime(amount);
    }

    [UFunction(FunctionFlags.BlueprintCallable | FunctionFlags.Exec)]
    public void RemoveSlime(float amount)
    {
        OwningCharacter.SlimeInventory.AddSlime(-amount);
    }
    
    [UFunction(FunctionFlags.BlueprintCallable | FunctionFlags.Exec)]
    public void KillAllEnemies()
    {
        UGameplayStatics.GetAllActorsOfClass(typeof(ABKSlimeCharacter), out IList<ABKSlimeCharacter> foundActors);
        
        for (int i = 0; i < foundActors.Count; i++)
        {
            ABKSlimeCharacter slime = foundActors[i];
            slime.HealthComponent.ApplyDamage(slime.HealthComponent.CurrentValue);
        }
    }
    
    ABKCharacter OwningCharacter
    {
        get
        {
            ABKCharacter character = (ABKCharacter) PlayerController.ControlledPawn;
            return character;
        } 
    }
}