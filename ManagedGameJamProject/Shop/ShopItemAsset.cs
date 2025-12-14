using ManagedGameJamProject.Item;
using UnrealSharp.Attributes;
using UnrealSharp.Core;
using UnrealSharp.Core.Attributes;
using UnrealSharp.CoreUObject;
using UnrealSharp.UnrealSharpCore;

namespace ManagedGameJamProject.Shop;

[UClass]
public partial class UShopItemAsset : UCSPrimaryDataAsset
{
    public UShopItemAsset()
    {
        AssetName = "ShopItem";
    }
    
    [UProperty(PropertyFlags.EditAnywhere | PropertyFlags.BlueprintReadOnly)]
    public partial FText ItemName { get; set; }
    
    // Game jam solution deluxe, just write what the item does in the description.
    // Would be better to have a proper description system.
    [UProperty(PropertyFlags.EditAnywhere | PropertyFlags.BlueprintReadOnly)]
    public partial FText Description { get; set; }
    
    [UProperty(PropertyFlags.EditAnywhere | PropertyFlags.BlueprintReadOnly), UMetaData("AllowedClasses", "/Script/Engine.Texture,/Script/Engine.MaterialInterface,/Script/Engine.SlateTextureAtlasInterface"), UMetaData("DisallowedClasses", "/Script/MediaAssets.MediaTexture")]
    public partial UObject Icon { get; set; }
    
    [UProperty(PropertyFlags.EditAnywhere | PropertyFlags.BlueprintReadOnly)]
    public partial float Price { get; set; }
    
    [UProperty()]
    public partial bool HasQuantity { get; set; }
    
    [UProperty(PropertyFlags.EditAnywhere | PropertyFlags.BlueprintReadOnly), UMetaData("EditCondition", "HasQuantity")]
    public partial int Quantity { get; set; }
    
    [UProperty(PropertyFlags.EditAnywhere | PropertyFlags.BlueprintReadOnly | PropertyFlags.Instanced)]
    public partial UConsumable Consumable { get; set; }
}