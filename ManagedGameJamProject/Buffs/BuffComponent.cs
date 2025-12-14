using UnrealSharp;
using UnrealSharp.Attributes;
using UnrealSharp.Engine;

namespace ManagedGameJamProject.Buffs;

[UClass]
public partial class UBuffComponent : UActorComponent
{
    [UProperty(PropertyFlags.BlueprintReadOnly)]
    public partial IList<UBuff> ActiveBuffs { get; set; }
    
    public void ApplyBuff(TSubclassOf<UBuff> buff, bool delayedStart = false)
    {
        UBuff buffInstance = NewObject(this, buff);
        buffInstance.Initialize(Owner, this, delayedStart);
        ActiveBuffs.Add(buffInstance);
    }
    
    public bool HasBuffOfClass(TSubclassOf<UBuff> buffClass)
    {
        foreach (UBuff buff in ActiveBuffs)
        {
            if (buff.Class == buffClass)
            {
                return true;
            }
        }
        
        return false;
    }
    
    public void RemoveBuff(UBuff buff)
    {
        if (ActiveBuffs.Contains(buff))
        {
            ActiveBuffs.Remove(buff);
        }
    }
}