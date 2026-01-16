using UnrealSharp.AIModule;
using UnrealSharp.Attributes;
using UnrealSharp.CoreUObject;
using UnrealSharp.Engine;

namespace ManagedGameJamProject.AI.Tasks;

[UClass]
public partial class UAreaOfDamageTask : UBTTask_BlueprintBase
{
    [UProperty(PropertyFlags.EditAnywhere | PropertyFlags.BlueprintReadOnly)]
    public partial float DamageAmount { get; set; }
    
    [UProperty(PropertyFlags.EditAnywhere | PropertyFlags.BlueprintReadOnly)]
    public partial FBlackboardKeySelector TargetLocationKey { get; set; }
    
    [UProperty(PropertyFlags.EditInstanceOnly)]
    public partial FBlackboardKeySelector RadiusKey { get; set; }
    
    public override void ReceiveExecuteAI(AAIController ownerController, APawn controlledPawn)
    {
        base.ReceiveExecuteAI(ownerController, controlledPawn);
        FVector center = UBTFunctionLibrary.GetBlackboardValueAsVector(this, TargetLocationKey);
        float damageRadius = UBTFunctionLibrary.GetBlackboardValueAsFloat(this, RadiusKey);
        ApplyAreaDamage(center, damageRadius, DamageAmount, controlledPawn);
        FinishExecute(true);
    }
    
    public static void ApplyAreaDamage(FVector center, float radius, float damageAmount, APawn controlledPawn)
    {
        SystemLibrary.MultiSphereTraceByChannel(center, center, radius, ETraceChannel.Visibility.ToTraceQuery(), false, new List<AActor>() { controlledPawn }, EDrawDebugTrace.None, out IList<FHitResult> outHits, false);
        
        foreach (FHitResult hit in outHits)
        {
            AActor? hitActor = hit.Actor;
            if (hitActor == null)
            {
                continue;
            }
            
            UBKHealthComponent healthComponent = hitActor.GetComponentByClass<UBKHealthComponent>();
            if (healthComponent == null)
            {
                continue;
            }
            
            healthComponent.ApplyDamage(damageAmount);
        }
    }
}