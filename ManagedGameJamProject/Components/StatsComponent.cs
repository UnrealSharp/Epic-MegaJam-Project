using UnrealSharp;
using UnrealSharp.Attributes;
using UnrealSharp.Engine;
using UnrealSharp.GameplayTags;

namespace ManagedGameJamProject.Components;

[UMultiDelegate]
public delegate void OnStatsChangedDelegate(FGameplayTag statTag, float newValue, float oldValue);

[UClass]
public partial class UStatsComponent : UActorComponent
{
    private readonly IDictionary<FGameplayTag, float> _stats = new Dictionary<FGameplayTag, float>();
    
    [UProperty(PropertyFlags.BlueprintAssignable)]
    public partial TMulticastDelegate<OnStatsChangedDelegate> OnStatsChanged { get; set; }
    
    public void SetStat(FGameplayTag statTag, float value)
    {
        float oldValue = GetStat(statTag);
        _stats[statTag] = value;
        OnStatsChanged.Invoke(statTag, value, oldValue);
    }
     
    public float GetStat(FGameplayTag statTag)
    {
        if (_stats.TryGetValue(statTag, out float value))
        {
            return value;
        }
        
        return 0f;
    }
    
    public bool TryGetStat(FGameplayTag statTag, out float value)
    {
        return _stats.TryGetValue(statTag, out value);
    }
    
    public void ModifyStat(FGameplayTag statTag, float delta)
    {
        float currentValue = GetStat(statTag);
        SetStat(statTag, currentValue + delta);
    }
}