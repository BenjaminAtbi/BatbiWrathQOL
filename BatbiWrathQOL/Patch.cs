using Kingmaker;
using Kingmaker.PubSubSystem;
using Kingmaker.View;
using Kingmaker.Visual;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UI.MVVM._VM.ActionBar;
using HarmonyLib;
using UnityEngine;
using Kingmaker.Utility;
using Kingmaker.Blueprints;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using System;
using UniRx;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Visual.CharacterSystem;
using System.Linq;
using Kingmaker.ResourceLinks;
using Kingmaker.Blueprints.JsonSystem;
using System.ComponentModel;
using Kingmaker.Blueprints.CharGen;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.Inspect;
using Kingmaker.UI.MVVM._VM.Inspect;


namespace BatbiWrathQOL
{
    //**********************************
    // Remove green outline
    //**********************************
    [HarmonyPatch(typeof(UnitEntityView), nameof(UnitEntityView.UpdateHighlight))]
    internal static class UnitEntityView_UpdateHighlight_Patch
    {
        static bool Prefix(UnitEntityView __instance, ref UnitMultiHighlight ___m_Highlighter, bool raiseEvent = true)
        {
            if (!Main.Enabled || __instance.EntityData == null) return true;

            if (!__instance.MouseHighlighted && !__instance.DragBoxHighlighted && !__instance.EntityData.Descriptor.State.IsDead && !__instance.EntityData.IsPlayersEnemy &&
                __instance.EntityData.IsPlayerFaction)
            {
                ___m_Highlighter.BaseColor = Color.clear;

                if (raiseEvent)
                {
                    EventBus.RaiseEvent<IUnitHighlightUIHandler>(delegate (IUnitHighlightUIHandler h)
                    {
                        h.HandleHighlightChange(__instance);
                    });
                }
                return false;
            }
            return true;
        }
    }

    //**********************************
    // Inspect Friendly Units
    //**********************************
    //[HarmonyPatch(typeof(InspectUnitsHelper), nameof(InspectUnitsHelper.IsInspectAllow))]
    //internal static class InspectUnitsHelper_IsInspectAllow_Patch
    //{
    //    static void Prefix(ref bool __result, UnitEntityData unit)
    //    {
    //        if (!Main.Enabled) return;

    //        __result = __result || (!(unit == null) && unit.IsPlayerFaction);
    //    }
    //}

    //[HarmonyPatch(typeof(InGameInspectVM), nameof(InGameInspectVM.OnUnitHover))]
    //internal static class InGameInspectVM_OnUnitHover_Patch
    //{
    //    [HarmonyTranspiler]
    //    static IEnumerable<CodeInstruction> OnUnitHoverTranspile(IEnumerable<CodeInstruction> instructions)
    //    {
    //        Main.DebugLog("entering function");
    //        int anchor = -1;
    //        var operand = "get_IsPlayersEnemy";
    //        for (var i = 0; i < instructions.Count(); i++)
    //        {
    //            if (i < instructions.Count()-5 && instructions.ElementAt(i).opcode == OpCodes.Ldarg_1 &&
    //                //instructions.ElementAt(i + 2).opcode == OpCodes.Callvirt && instructions.ElementAt(i).operand as string == operand &&
    //                instructions.ElementAt(i + 4).opcode == OpCodes.Ret)
    //            {
    //                anchor = i;
    //            }
    //            var instr = i < anchor + 5 ? new CodeInstruction(OpCodes.Nop) : instructions.ElementAt(i);
    //            Main.DebugLog(instr.opcode.ToString());
    //            yield return instr;
    //        }

    //        if (anchor <0) Main.DebugLog("cannot find onunitchanged hover instructions");
    //    }
    //}

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


    //
    [HarmonyPatch(typeof(BlueprintsCache))]
    public class BlueprintsCache_Patches
    {
        [HarmonyPriority(Priority.First)]
        [HarmonyPatch(nameof(BlueprintsCache.Init)), HarmonyPostfix]

        static void Postfix()
        {
            HolyBombFix();
            LichSkinPatch();
            //SkinUnlockerPatch();
        }


        public static void HolyBombFix()
        {
            try
            {
                var HolyBomb = ResourcesLibrary.TryGetBlueprint<BlueprintAbility>("b94ee802dc1574b4fb71215a4a6f11dc");
                var mainTargetConditional = HolyBomb.Components.OfType<AbilityEffectRunAction>().First().Actions.Actions.OfType<Conditional>().First();
                var evilConditional = mainTargetConditional.IfTrue.Actions.OfType<Conditional>().First().IfFalse.Actions.OfType<Conditional>().First();
                var reanimatorConditional = evilConditional.IfTrue.Actions.OfType<Conditional>().First();
                reanimatorConditional.IfTrue.Actions.OfType<ContextActionDealDamage>().First().Half = false;
                reanimatorConditional.IfFalse.Actions.OfType<ContextActionDealDamage>().First().Half = false;
            }
            catch (Exception e)
            {
                Main.DebugError(e);
            }
        }



        class RampNameCompare : IEqualityComparer<Texture2D>
        {
            public bool Equals(Texture2D a, Texture2D b)
            {
                return a.name.Split('_')[2].Equals(b.name.Split('_')[2]);
            }

            public int GetHashCode(Texture2D obj)
            {
                return obj.name.Split('_')[2].GetHashCode();
            }
        }

        public static void SkinUnlockerPatch()
        {
            var raceBodies = new Dictionary<Race, List<EquipmentEntity>>();
            var raceHeads = new Dictionary<Race, List<EquipmentEntity>>();
            var raceRamps = new Dictionary<Race, List<Texture2D>>();

            var racecodes = new List<String>() {
                "0a5d473ead98b0646b94495af250fdc4",
                "5c4e42124dc2b4647af6e36cf2590500",
                "64e8b7d5f1ae91d45bbf1e56a3fdff01"};

            foreach (var racePreset in racecodes.Select(c => ResourcesLibrary.TryGetBlueprint<BlueprintRace>(c)))
            {
                var entityLinks = new List<EquipmentEntityLink>();
                Race racePresetRace;

                foreach (var gender in new Gender[] { Gender.Male, Gender.Female })
                {
                    //entityLinks.AddRange(racePreset.Skin.GetLinks(gender, racePreset.RaceId));
                }

                if (Enum.TryParse(racePreset.name.Split('_')[0], out racePresetRace))
                {
                    if (raceBodies.ContainsKey(racePresetRace))
                    {
                        raceBodies.Get(racePresetRace).AddRange(entityLinks.Select(e => e.Load()));
                    }
                    else raceBodies.Add(racePresetRace, entityLinks.Select(e => e.Load()).ToList());
                }
            }

            foreach (var race in Game.Instance.BlueprintRoot.Progression.CharacterRaces)
            {
                var heads = (race.MaleOptions.Heads.Concat(race.FemaleOptions.Heads)).Select(h => h.Load() as EquipmentEntity);

                raceHeads.Add(race.RaceId, heads.ToList());
                raceRamps.Add(race.RaceId, heads.First().PrimaryColorsProfile.Ramps.ToList());
            }

            var races = (IEnumerable<Race>)Enum.GetValues(typeof(Race));
            List<Texture2D> allRamps = races.Where(r => raceRamps.ContainsKey(r)).SelectMany(r => raceRamps.Get(r)).Distinct(new RampNameCompare()).ToList();
            allRamps.Sort((a, b) => a.name.Split('_')[2].CompareTo(b.name.Split('_')[2]));

            foreach (var race in races)
            {
                if (raceHeads.ContainsKey(race) && raceBodies.ContainsKey(race))
                {
                    var currentRamps = raceHeads.Get(race).First().PrimaryRamps;
                    var unifiedRamps = currentRamps.Concat(allRamps.Except(currentRamps)).ToList();
                    //var primaryProfile = new CharacterColorsProfile { PrimaryRamps = unifiedRamps, SecondaryRamps = raceHeads.Get(race).First().SecondaryRamps };

                    foreach (var head in raceHeads.Get(race))
                    {
                        head.PrimaryColorsProfile.Ramps = unifiedRamps;
                    }

                    foreach (var part in raceBodies.Get(race))
                    {
                        part.PrimaryColorsProfile.Ramps = unifiedRamps;
                    }
                }
            }
        }

        public static void LichSkinPatch()
        {
            try
            {
                //var LichDesaturation = ResourcesLibrary.TryGetBlueprint<KingmakerEquipmentEntity>("a4356509220f4bf19f05f70eb5db9240");
                //Main.DebugLog($"loaded lich {(LichDesaturation == null ? "null" : LichDesaturation.GetType().ToString())}");
                var DhampirRace = ResourcesLibrary.TryGetBlueprint<BlueprintRace>("64e8b7d5f1ae91d45bbf1e56a3fdff01");
                //Main.DebugLog($"loaded Dhampir {(DhampirRace == null ? "null" : DhampirRace.GetType().ToString())}");

                //Main.DebugLog($"ramps {String.Join(",",DhampirRace.FemaleOptions.Heads.First().Load().PrimaryRamps)}");
                //var DhampirRamps = DhampirRace.MaleOptions.Heads.First().Load().PrimaryRamps;

                //var Lich = LichDesaturation.m_FemaleArray.First().Load();

                //foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(Lich))
                //{
                //    string name = descriptor.Name;
                //    object value = descriptor.GetValue(Lich);
                //    Main.DebugLog($"{name}={value}");
                //}

                //LichDesaturation.m_FemaleArray.First().Load().OutfitParts = null;
                //LichDesaturation.m_MaleArray.First().Load().OutfitParts = null;

                //Main.DebugLog($"name: {Lich.name},layer: {Lich.Layer},body parts: {Lich.BodyParts.Count},outfitparts: {Lich.OutfitParts.Count}, Bakedtextures: {Lich.BakedTextures}," +
                //    $"Can't be hidden: {Lich.CantBeHiddenByDollRoom}, color presets: {Lich.ColorPresets}, primary count: {Lich.m_PrimaryRamps.Count}, " +
                //    $"Secondary count: {Lich.m_SecondaryRamps.Count}, special prim count: {Lich.m_SpecialPrimaryRamps.Count}, special sec count: {Lich.m_SpecialSecondaryRamps.Count}," +
                //    $"prim color profile:{Lich.PrimaryColorsProfile}, sec color profile:{Lich.SecondaryColorsProfile},");

                //Main.DebugLog($"lich: {LichEE.name}, {LichEE.m_PrimaryRamps.Count}, {LichEE.m_SecondaryRamps.Count}, {LichEE.m_SpecialPrimaryRamps.Count}, {LichEE.m_SpecialSecondaryRamps.Count}");
                //LichDesaturation.m_FemaleArray.First().Load().PrimaryColorsProfile.Ramps = DhampirRamps;
                //LichDesaturation.m_MaleArray.First().Load().PrimaryColorsProfile.Ramps = DhampirRamps;


                var LichVisuals = ResourcesLibrary.TryGetBlueprint<BlueprintClassAdditionalVisualSettings>("6a60a86905b24137ae1e4300b6ab0841");

                LichVisuals.CommonSettings.m_EquipmentEntities = new KingmakerEquipmentEntityReference[] { };
                LichVisuals.ColorRamps = new BlueprintClassAdditionalVisualSettings.ColorRamp[] { };

                var skinTypeCodes = new HashSet<long>()
                {
                    256L, 512L, 2048L,
                };

                //LichVisuals.ColorRamps = LichVisuals.ColorRamps.Where(c => skinTypeCodes.Contains(c.m_Type)).ToArray();

                //Main.DebugLog($"lich ramps: {String.Join(" ", LichVisuals.ColorRamps.Select(c => c.m_Type))}");
                //;
                //foreach (var colorramp in LichVisuals.ColorRamps.Where(c => !skinTypeCodes.Contains(c.m_Type)))
                //{
                //    colorramp.m_Primary = setrampval(colorramp.Primary);
                //    colorramp.m_Secondary = setrampval(colorramp.Secondary);
                //    colorramp.m_SpecialPrimary = setrampval(colorramp.SpecialPrimary);
                //    colorramp.m_SpecialSecondary = setrampval(colorramp.SpecialSecondary);
                //}

                //var LichVisuals2 = ResourcesLibrary.TryGetBlueprint<BlueprintClassAdditionalVisualSettings>("6a60a86905b24137ae1e4300b6ab0841");



            }
            catch (Exception e)
            {
                Main.DebugError(e);
            }

        }

        private static int setrampval(int value)
        {
            return value == 3 ? 0 : value;
        }
    }

}
