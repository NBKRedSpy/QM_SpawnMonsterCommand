using System;

namespace SpawnMonsterCommand_Bootstrap;

/// <summary>
/// Indicates a function has copied logic from the game's original method.  The game's method that is indicated should be 
/// checked for compatiblity.
/// Note, this attribute is used by the HarmonyPatchChangeParser tool to identify patches that may need to be updated.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false)]
public class CopyWarningAttribute : Attribute
{
    public Type OriginalType { get; }
    public string OriginalMethodName { get; }
    public string Message { get; }

    public CopyWarningAttribute(Type originalType, string originalMethodName, string message = "")
    {
        OriginalType = originalType;
        OriginalMethodName = originalMethodName;
        Message = message;
    }
}