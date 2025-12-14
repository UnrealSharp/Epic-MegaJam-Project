using UnrealSharp;
using UnrealSharp.Attributes;
using UnrealSharp.Engine;

namespace ManagedGameJamProject.Components;

[UClass]
public partial class USlimeMeshComponent : UStaticMeshComponent
{
    [UProperty]
    public partial UMaterialInstanceDynamic SlimeMaterialInstance { get; set; }

    public void ApplySlimeMaterial()
    {
        SlimeMaterialInstance = CreateDynamicMaterialInstance(0, GetMaterial(0));
        SetMaterial(0, SlimeMaterialInstance);
        SlimeMaterialInstance.SetScalarParameterValue("RandomOffset", MathLibrary.RandomFloatInRange(0.0f, 1.0f).ToFloat());
    }
}