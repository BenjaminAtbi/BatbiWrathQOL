using HarmonyLib;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Inspect;
using Kingmaker.UI.MVVM._VM.Inspect;
using Kingmaker.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatbiWrathQOL.Patches
{
    ////**********************************
    //// Inspect Friendly Units
    ////**********************************
    //[HarmonyPatch(typeof(InspectUnitsHelper), nameof(InspectUnitsHelper.IsInspectAllow))]
    //internal static class InspectUnitsHelper_IsInspectAllow_Patch
    //{
    //    static bool Prefix(ref bool __result, UnitEntityData unit)
    //    {
    //        if (!Main.Enabled) return true;
    //        Main.DebugLog("entering IsInspectAllow");

    //        __result = __result || (!(unit == null) && unit.IsPlayerFaction);
    //        return false;
    //    }
    //}

    ////[HarmonyPatch(typeof(InGameInspectVM), nameof(InGameInspectVM.OnUnitHover))]
    ////internal static class InGameInspectVM_OnUnitHover_Patch
    ////{
    ////    [HarmonyTranspiler]
    ////    static IEnumerable<CodeInstruction> OnUnitHoverTranspile(IEnumerable<CodeInstruction> instructions)
    ////    {
    ////        var found = false;
    ////        Main.DebugLog("entering on hover function" + instructions.Count());
    ////        for (var i = 0; i < instructions.Count();)
    ////        {
    ////            if (i == 12
    ////                && instructions.ElementAt(i).opcode == OpCodes.Dup
    ////                && instructions.ElementAt(i + 1).opcode == OpCodes.Brtrue_S
    ////                && instructions.ElementAt(i + 2).opcode == OpCodes.Pop
    ////                && instructions.ElementAt(22).opcode == OpCodes.Ldarg_0
    ////                && instructions.ElementAt(22).labels.Count > 0)
    ////            {
    ////                found = true;
    ////                var label = instructions.ElementAt(22).labels.Get(0);
    ////                var a = new CodeInstruction(OpCodes.Brtrue_S, label);
    ////                Main.DebugLog(a.ToString());
    ////                yield return a;
    ////                i += 3;
    ////                Main.DebugLog("performing substitution");
    ////            }
    ////            else
    ////            {
    ////                Main.DebugLog(instructions.ElementAt(i).ToString());
    ////                yield return instructions.ElementAt(i);
    ////                i += 1;
    ////            }
    ////        }
    ////        if (!found) Main.DebugLog("cannot find code pattern in ActionBarVM.OnUnitChanged");
    ////    }
    ////}


    ///*
    // * Function that checks if unit is Player's Enemy always returns true, so Inspect will always activate
    // */
    //[HarmonyPatch(typeof(InGameInspectVM), nameof(InGameInspectVM.OnUnitHover))]
    //internal static class InGameInspectVM_OnUnitHover_Patch
    //{
    //    private static bool IsPlayersEnemyStub(UnitEntityData _) => true;

    //    [HarmonyTranspiler]
    //    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    //    {
    //        Main.DebugLog("Transpile: InGameInspectVM_OnUnitHover");
    //        var get_IsPlayersEnemy_PropertyGetter = AccessTools.PropertyGetter(typeof(UnitEntityData), nameof(UnitEntityData.IsPlayersEnemy));
    //        foreach (var instruction in instructions)
    //        {
    //            if (instruction.Calls(get_IsPlayersEnemy_PropertyGetter))
    //            {
    //                Main.DebugLog("InGameInspectVM_OnUnitHover Registered");
    //                instruction.operand = ((Func<UnitEntityData, bool>)IsPlayersEnemyStub).Method;
    //            }
    //            yield return instruction;
    //        }
    //    }
    //}

    ///*
    // * Function that checks if unit is Player's Enemy always returns true, so Inspect will always activate
    // */
    //[HarmonyPatch(typeof(InGameInspectVM), nameof(InGameInspectVM.OnUnitRightClick))]
    //internal static class InGameInspectVM_OnUnitRightClick_Patch
    //{
    //    private static bool IsPlayersEnemySub(UnitEntityData _) => true;

    //    static bool Prefix(UnitEntityView unitEntityView)
    //    {
    //        Main.DebugLog($"InGameInspectVM_OnRightClick - {unitEntityView.EntityData.CharacterName} - {Environment.StackTrace}");
    //        return true;
    //    }


    //    [HarmonyTranspiler]
    //    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    //    {

    //        Main.DebugLog("Transpile: InGameInspectVM_OnRightClick");
    //        var get_IsPlayersEnemy_PropertyGetter = AccessTools.PropertyGetter(typeof(UnitEntityData), nameof(UnitEntityData.IsPlayersEnemy));
    //        foreach (var instruction in instructions)
    //        {
    //            if (instruction.Calls(get_IsPlayersEnemy_PropertyGetter))
    //            {
    //                Main.DebugLog("InGameInspectVM_OnRightClick Registered");
    //                instruction.operand = ((Func<UnitEntityData, bool>)IsPlayersEnemySub).Method;
    //            }
    //            yield return instruction;
    //        }
    //    }
    //}

    ///*
    //* Function that checks if unit is Player's Enemy always returns true, so Inspect will always activate
    //*/
    ////[HarmonyPatch(typeof(InGameInspectVM), nameof(InGameInspectVM.OnUnitInspectRequest))]
    ////internal static class InGameInspectVM_OnUnitInspectRequest_Patch
    ////{
    ////    private static bool IsPlayersEnemySub(UnitEntityData _) => true;

    ////    static bool Prefix(UnitEntityView unitEntityView)
    ////    {
    ////        Main.DebugLog($"InGameInspectVM_OnUnitInspectRequest - {unitEntityView.EntityData.CharacterName}");
    ////        return true;
    ////    }

    ////    [HarmonyTranspiler]
    ////    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    ////    {
    ////        Main.DebugLog("Transpile: InGameInspectVM_OnUnitInspectRequest");
    ////        var get_IsPlayersEnemy_PropertyGetter = AccessTools.PropertyGetter(typeof(UnitEntityData), nameof(UnitEntityData.IsPlayersEnemy));
    ////        foreach (var instruction in instructions)
    ////        {
    ////            if (instruction.Calls(get_IsPlayersEnemy_PropertyGetter))
    ////            {
    ////                Main.DebugLog("InGameInspectVM_OnUnitInspectRequest Registered");
    ////                instruction.operand = ((Func<UnitEntityData, bool>)IsPlayersEnemySub).Method;
    ////            }
    ////            yield return instruction;
    ////        }
    ////    }
    ////}

    ///*
    // * Testing patchs
    // */
    //[HarmonyPatch(typeof(InGameInspectVM), nameof(InGameInspectVM.OnUnitTacticalCombatRightClick))]
    //internal static class InGameInspectVM_OnUnitTacticalCombatRightClick_Patch
    //{
    //    static bool Prefix(UnitEntityData unitEntityData)
    //    {
    //        Main.DebugLog($"InGameInspectVM_OnUnitTacticalCombatRightClick - {unitEntityData.CharacterName}");
    //        return true;
    //    }
    //}

    ///*
    // * Used by army combat
    // */
    //[HarmonyPatch(typeof(TacticalCombatInspectVM), nameof(TacticalCombatInspectVM.OnUnitRightClick))]
    //internal static class TacticalCombatInspectVM_OnUnitRightClick_Patch
    //{
    //    static bool Prefix(UnitEntityView unitEntityView)
    //    {
    //        Main.DebugLog($"TacticalCombatInspectVM_OnUnitRightClick - {unitEntityView.EntityData.CharacterName}");
    //        return true;
    //    }
    //}

    ///*
    //* Used by army combat
    //*/
    //[HarmonyPatch(typeof(TacticalCombatInspectVM), nameof(TacticalCombatInspectVM.OnUnitTacticalCombatRightClick))]
    //internal static class TacticalCombatInspectVM_OnUnitTacticalCombatRightClick_Patch
    //{
    //    static bool Prefix(UnitEntityData unitEntityData)
    //    {
    //        Main.DebugLog($"TacticalCombatInspectVM_OnUnitTacticalCombatRightClick - {unitEntityData.CharacterName}");
    //        return true;
    //    }
    //}
}
