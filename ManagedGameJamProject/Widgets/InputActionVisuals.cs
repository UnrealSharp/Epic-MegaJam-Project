using UnrealSharp.Attributes;
using UnrealSharp.Engine;
using UnrealSharp.EnhancedInput;
using UnrealSharp.UnrealSharpCore;

namespace ManagedGameJamProject.Widgets;

[UEnum]
public enum EInputActionDescriptionLocation : byte
{
    Left = 0,
    Right = 1,
}

[UClass]
public partial class UInputActionVisuals : UCSPrimaryDataAsset
{
    public UInputActionVisuals()
    {
        AssetName = "InputActionVisuals";
    }
    
    [UProperty(PropertyFlags.EditAnywhere | PropertyFlags.BlueprintReadOnly)]
    public partial UInputAction InputAction { get; set;  }
    
    [UProperty(PropertyFlags.EditAnywhere | PropertyFlags.BlueprintReadOnly)]
    public partial UTexture2D Icon { get; set;  }
}