using UnrealSharp;
using UnrealSharp.AIModule;
using UnrealSharp.Attributes;
using UnrealSharp.Core.Attributes;
using UnrealSharp.Engine;
using UnrealSharp.GameplayTags;
using UnrealSharp.UnrealSharpCore;

namespace ManagedGameJamProject.AI;

[UClass]
public partial class UResourceMeshListAsset : UCSPrimaryDataAsset
{
    [UProperty(PropertyFlags.EditDefaultsOnly | PropertyFlags.BlueprintReadOnly)]
    public partial IList<UStaticMesh> ResourceMeshes { get; set; }
    
    public UStaticMesh? GetRandomMesh()
    {
        if (ResourceMeshes.Count == 0)
        {
            return null;
        }
        
        int randomIndex = MathLibrary.RandomIntegerInRange(0, ResourceMeshes.Count - 1);
        return ResourceMeshes[randomIndex];
    }
}

[UClass]
public partial class UBKAIBehaviorAsset : UCSPrimaryDataAsset
{
    public UBKAIBehaviorAsset()
    {
        AssetName = "Behavior";
        ChanceToSpawnLongDistanceSlime = 0.5f;
    }
    
    [UProperty(PropertyFlags.EditDefaultsOnly | PropertyFlags.BlueprintReadOnly)]
    public partial UBehaviorTree BehaviorTree { get; set; }
    
    [UProperty(PropertyFlags.EditDefaultsOnly | PropertyFlags.BlueprintReadOnly)]
    public partial UStaticMesh SlimeMesh { get; set; }
    
    [UProperty(PropertyFlags.EditDefaultsOnly | PropertyFlags.BlueprintReadOnly)]
    public partial FGameplayTag SlimeTypeTag { get; set; }
    
    [UProperty(PropertyFlags.EditDefaultsOnly | PropertyFlags.BlueprintReadOnly)]
    public partial UResourceMeshListAsset? ResourceMesh { get; set; }
    
    [UProperty(PropertyFlags.EditDefaultsOnly | PropertyFlags.BlueprintReadOnly)]
    public partial IList<UMaterialInterface> ResourceMaterials { get; set; }
    
    [UProperty(PropertyFlags.EditDefaultsOnly | PropertyFlags.BlueprintReadOnly)]
    public partial TSubclassOf<ABKSlimeCharacter> SlimeClass { get; set; }
    
    [UProperty(PropertyFlags.EditDefaultsOnly | PropertyFlags.BlueprintReadOnly)]
    public partial float MoveSpeed { get; set; }
    
    [UProperty(PropertyFlags.EditDefaultsOnly | PropertyFlags.BlueprintReadOnly), UMetaData("ClampMin", "0.0")]
    public partial float MinStartSlimeAmount { get; set; }
    
    [UProperty(PropertyFlags.EditDefaultsOnly | PropertyFlags.BlueprintReadOnly), UMetaData("ClampMin", "0.0")]
    public partial float MaxStartSlimeAmount { get; set; }
    
    [UProperty(PropertyFlags.EditDefaultsOnly | PropertyFlags.BlueprintReadOnly), UMetaData("ClampMin", "0.0"), UMetaData("ClampMax", "1.0")]
    public partial float ChanceToSpawnLongDistanceSlime { get; set; }
    
    [UFunction(FunctionFlags.BlueprintCallable)]
    public void ApplyRandomMesh(UStaticMeshComponent meshComponent)
    {
        if (ResourceMesh == null)
        {
            return;
        }

        UStaticMesh? mesh = ResourceMesh.GetRandomMesh();
        if (mesh == null)
        {
            return;
        }
        
        meshComponent.SetStaticMesh(mesh);

        if (ResourceMaterials.Count <= 0)
        {
            return;
        }
        
        int randomIndex = MathLibrary.RandomIntegerInRange(0, ResourceMaterials.Count - 1);
        UMaterialInterface material = ResourceMaterials[randomIndex];
        meshComponent.SetMaterial(0, material);
    }
}