using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Inspect;
using Kingmaker.UI.MVVM._VM.Inspect;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Buffs;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Parts;
using Kingmaker.Utility;
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
    /*
     * Function that checks if unit is Player's Enemy always returns true, so Inspect will always activate
     */
    [HarmonyPatch(typeof(InGameInspectVM), nameof(InGameInspectVM.OnUnitRightClick))]
    internal static class InGameInspectVM_OnUnitRightClick_Patch
    {


        static bool Prefix(UnitEntityView unitEntityView)
        {
            var RepurposeBlueprint = ResourcesLibrary.TryGetBlueprint<BlueprintBuff>("5e18ce2e21330e34690c372fbd9d6d60");
            try
            {
                    Main.DebugLog($"{unitEntityView.EntityData.CharacterName} \n {String.Join("\n", unitEntityView.EntityData.Buffs.Enumerable.Select(b => $"{b.Name} - {b.Blueprint.AssetGuid} - notdispellable: {b.IsNotDispelable}"))}");
            }
            catch (Exception ex)
            {
                Main.DebugLog(ex.ToString());
            }
            
            return true;
        }

    }

    //[HarmonyPatch(typeof(ContextActionResurrect), nameof(ContextActionResurrect.RunAction))]
    //internal static class ContextActionResurrect_RunAction_Patch
    //{
    //    static bool Prefix()
    //    {
    //        Main.DebugLog($"ContextActionResurrect Run Action Pre");
    //        return true;
    //    }

    //    static void Postfix()
    //    {

    //        Main.DebugLog($"ContextActionResurrect Run Action Post");
    //    }

    //}

    //[HarmonyPatch(typeof(ChangeFaction), nameof(ChangeFaction.OnTurnOn))]
    //internal static class ChangeFaction_OnTurnOn_Patch
    //{
    //    static void Prefix(ChangeFaction __instance)
    //    {

    //        Main.DebugLog($"Change Faction OnTurnOn Prefix: {__instance.Owner.CharacterName} - allowdirectcontrol: {__instance.m_AllowDirectControl} - counter: {__instance.Owner.PreventDirectControl}");
    //    }

    //    static void Postfix(ChangeFaction __instance)
    //    {
    //        Main.DebugLog($"Change Faction OnTurnOn Postfix: {__instance.Owner.CharacterName} - allowdirectcontrol: {__instance.m_AllowDirectControl} - counter: {__instance.Owner.PreventDirectControl}");
    //    }
    //}

    //[HarmonyPatch(typeof(ChangeFaction), nameof(ChangeFaction.OnTurnOff))]
    //internal static class ChangeFaction_OnTurnOff_Patch
    //{
    //    static void Prefix(ChangeFaction __instance)
    //    {

    //        Main.DebugLog($"Change Faction OnTurnOff Prefix: {__instance.Owner.CharacterName} - allowdirectcontrol: {__instance.m_AllowDirectControl} - counter: {__instance.Owner.PreventDirectControl}");
    //    }

    //    static void Postfix(ChangeFaction __instance)
    //    {
    //        Main.DebugLog($"Change Faction OnTurnOff Postfix: {__instance.Owner.CharacterName} - allowdirectcontrol: {__instance.m_AllowDirectControl} - counter: {__instance.Owner.PreventDirectControl}");
    //    }
    //}


    /*
    * Function that checks if unit is Player's Enemy always returns true, so Inspect will always activate
    */
    //[HarmonyPatch(typeof(InGameInspectVM), nameof(InGameInspectVM.OnUnitInspectRequest))]
    //internal static class InGameInspectVM_OnUnitInspectRequest_Patch
    //{
    //    private static bool IsPlayersEnemySub(UnitEntityData _) => true;

    //    static bool Prefix(UnitEntityView unitEntityView)
    //    {
    //        Main.DebugLog($"InGameInspectVM_OnUnitInspectRequest - {unitEntityView.EntityData.CharacterName}");
    //        return true;
    //    }

    //    [HarmonyTranspiler]
    //    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    //    {
    //        Main.DebugLog("Transpile: InGameInspectVM_OnUnitInspectRequest");
    //        var get_IsPlayersEnemy_PropertyGetter = AccessTools.PropertyGetter(typeof(UnitEntityData), nameof(UnitEntityData.IsPlayersEnemy));
    //        foreach (var instruction in instructions)
    //        {
    //            if (instruction.Calls(get_IsPlayersEnemy_PropertyGetter))
    //            {
    //                Main.DebugLog("InGameInspectVM_OnUnitInspectRequest Registered");
    //                instruction.operand = ((Func<UnitEntityData, bool>)IsPlayersEnemySub).Method;
    //            }
    //            yield return instruction;
    //        }
    //    }
    //}

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
