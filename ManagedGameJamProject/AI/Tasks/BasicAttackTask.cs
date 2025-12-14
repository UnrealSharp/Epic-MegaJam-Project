using UnrealSharp.AIModule;
using UnrealSharp.Attributes;
using UnrealSharp.CoreUObject;
using UnrealSharp.Engine;

namespace ManagedGameJamProject.AI.Tasks;

[UClass]
public partial class UBasicAttackTask : UBTTask_BlueprintBase
{
    [UProperty(PropertyFlags.EditInstanceOnly)]
    public partial FBlackboardKeySelector TargetActor { get; set; }
    
    [UProperty(PropertyFlags.EditInstanceOnly)]
    public partial float DamageAmount { get; set; }
    
    public override void ReceiveExecute(AActor ownerActor)
    {
        base.ReceiveExecute(ownerActor);

        UObject target = UBTFunctionLibrary.GetBlackboardValueAsObject(this, TargetActor);
        if (target is not AActor actor)
        {
            FinishExecute(false);
            return;
        }

        UBKHealthComponent healthComponent = actor.GetComponentByClass<UBKHealthComponent>();
        healthComponent.ApplyDamage(DamageAmount);
        
        FinishExecute(true);
    }
}