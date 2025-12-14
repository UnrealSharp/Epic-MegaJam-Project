using UnrealSharp.Attributes;
using UnrealSharp.CoreUObject;
using UnrealSharp.Engine;
using UnrealSharp.GameJamProject;

namespace ManagedGameJamProject.Visuals;

[UClass]
public partial class ADangerZoneActor : AActor
{
    [UProperty(DefaultComponent = true, RootComponent = true)]
    public partial USceneComponent SceneRoot { get; set; }
    
    [UProperty(DefaultComponent = true, AttachmentComponent = "SceneRoot")]
    public partial UDecalComponent DangerZoneDecal { get; set; }
    
    public void SetDangerZoneSize(float size)
    {
        FVector sizeVector = new FVector(1000.0f, size, size);
        UGameJamExtensions.SetDecalSize(DangerZoneDecal, sizeVector);
    }
}