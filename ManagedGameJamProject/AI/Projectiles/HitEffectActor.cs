using UnrealSharp.Attributes;
using UnrealSharp.Engine;

namespace ManagedGameJamProject.AI.Projectiles;

[UClass]
public partial class AHitEffectActor : AActor
{
    [UProperty(DefaultComponent = true, RootComponent = true)]
    public partial USphereComponent HitDetectionSphere { get; set; }
    
    [UProperty(DefaultComponent = true)]
    public partial UStaticMeshComponent VisualMesh { get; set; }
    
    public void Initialize()
    {
        HitDetectionSphere.GetOverlappingActors(out IList<AActor> overlappingActors);
        OnHitDetectionSphereOverlapBegin(overlappingActors);
    }
    
    public virtual void OnHitDetectionSphereOverlapBegin(IList<AActor> otherActors)
    {
        
    }
}