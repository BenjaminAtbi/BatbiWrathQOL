using HarmonyLib;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UI.MVVM._VM.ActionBar;
using Kingmaker.Utility;
using System.Collections.Generic;
using System.Reflection.Emit;
using UniRx;
namespace BatbiWrathQOL.Patches
{


    //**********************************
    // Remember selected spell tab level 
    //**********************************
    static class UnitSpellLevels
    {
        public static Dictionary<int, int> saved = new Dictionary<int, int>();
        public static UnitEntityData? SelectedUnit;
    }

    [HarmonyPatch(typeof(ActionBarVM), MethodType.Constructor)]
    internal static class ActionBarVM_Patch
    {
        static void Postfix(ActionBarVM __instance)
        {
            __instance.CurrentSpellLevel.Subscribe(delegate (int value)
            {
                if (UnitSpellLevels.SelectedUnit != null)
                {
                    //Main.DebugLog($"lvl change: {UnitSpellLevels.SelectedUnit.CharacterName} {UnitSpellLevels.SelectedUnit.GetHashCode()} {value}");
                    UnitSpellLevels.saved[UnitSpellLevels.SelectedUnit.GetHashCode()] = value;
                }
            });
        }
    }

    [HarmonyPatch(typeof(ActionBarVM), nameof(ActionBarVM.OnUnitChanged))]
    internal static class ActionBarVM_OnUnitChanged_Patch
    {
        static int UnitChange(UnitEntityData unit)
        {
            if (unit != null)
            {
                UnitSpellLevels.SelectedUnit = unit;
                if (!UnitSpellLevels.saved.ContainsKey(unit.GetHashCode()))
                {
                    UnitSpellLevels.saved.Add(unit.GetHashCode(), 0);
                }
                //Main.DebugLog($"referencing data: {unit.CharacterName}, {UnitSpellLevels.saved.Get(unit.GetHashCode())}");
                return UnitSpellLevels.saved.Get(unit.GetHashCode());
            }
            return 0;
        }

        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> OnUnitChangedTranspile(IEnumerable<CodeInstruction> instructions)
        {
            var found = false;
            foreach (var instruction in instructions)
            {
                if (!found && instruction.opcode == OpCodes.Ldc_I4_0)
                {
                    found = true;
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Call,
                        SymbolExtensions.GetMethodInfo((UnitEntityData unit) => UnitChange(unit)));
                }
                else
                {
                    yield return instruction;
                }
            }
            if (!found) Main.DebugLog("cannot find Ldc_I4_0 in ActionBarVM.OnUnitChanged");
        }
    }
}
