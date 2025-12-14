using ManagedGameJamProject.Buffs;
using UnrealSharp.Attributes;
using UnrealSharp.Engine;

namespace ManagedGameJamProject.AI.Projectiles;

[UClass]
public partial class AGooHitEffect : AHitEffectActor
{
    public override void OnHitDetectionSphereOverlapBegin(IList<AActor> otherActors)
    {
        foreach (AActor actor in otherActors)
        {
            OnBuffApplied(actor);
        }
    }

    public override void ActorBeginOverlap(AActor otherActor)
    {
        OnBuffApplied(otherActor);
    }
    
    void OnBuffApplied(AActor actor)
    {
        UBuffComponent buffComponent = actor.GetComponentByClass<UBuffComponent>();
            
        if (buffComponent != null && !buffComponent.HasBuffOfClass(typeof(USlowMoBuff)))
        {
            buffComponent.ApplyBuff(typeof(USlowMoBuff));
        }
    }
}