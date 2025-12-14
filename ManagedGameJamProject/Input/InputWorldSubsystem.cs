using ManagedGameJamProject.Widgets;
using UnrealSharp.Attributes;
using UnrealSharp.Engine;
using UnrealSharp.EnhancedInput;
using UnrealSharp.UnrealSharpCore;

namespace ManagedGameJamProject.Input;

[UClass]
public partial class UInputWorldSubsystem : UCSWorldSubsystem
{
    [UProperty] 
    public partial IList<UInputActionVisuals> InputVisuals { get; set; }

    public override bool DoesSupportWorldType(ECSWorldType worldType)
    {
        return worldType == ECSWorldType.Game || worldType == ECSWorldType.PIE;
    }
    
    public async Task<UInputActionVisuals?> GetInputVisuals(UInputAction action)
    {
        if (InputVisuals.Count == 0)
        {
            await LoadVisuals();
        } 
        
        foreach (UInputActionVisuals visuals in InputVisuals)
        {
            if (ReferenceEquals(visuals.InputAction, action))
            {
                return visuals;
            }
        }

        return null;
    }

    async Task LoadVisuals()
    {
        UAssetManager assetManager = UAssetManager.Get();
        IList<UInputActionVisuals> visualsList = await assetManager.LoadPrimaryAssets<UInputActionVisuals>(AssetTypes.InputActionVisuals.PrimaryAssetList);
        
        for (int i = 0; i < visualsList.Count; i++)
        {
            InputVisuals.Add(visualsList[i]);
        }
    }
}