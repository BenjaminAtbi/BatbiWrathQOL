using HarmonyLib;
using Kingmaker.UI.MVVM._VM.ActionBar;
using Kingmaker.UnitLogic.Mechanics.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatbiWrathQOL.src.Patches
{
    internal class SummonToggles
    {
        
        /*
         * When spawning a summoned creature, initially toggle off combat abilities 
         */
        [HarmonyPatch(typeof(ContextActionSpawnMonster), nameof(ContextActionSpawnMonster.RunAction))]
        static class SummonsAbilitiesToggleOff
        {
            static void Prefix(ContextActionSpawnMonster __instance)
            {
                if (!Main.Enabled) return;
                __instance.AfterSpawn.Actions = __instance.AfterSpawn.Actions.Concat(new[] { new ContextActionToggleAbilities() }).ToArray();
            }
        }

        internal class ContextActionToggleAbilities : ContextAction
        {
            public override string GetCaption()
            {
                return "Initially toggle off combat abilities with To-hit drawbacks";
            }

            public override void RunAction()
            {
                if (Target.Unit.IsSummoned())
                {
                    foreach(var ability in Target.Unit.ActivatableAbilities){
                        if(ability.Blueprint.AssetGuidThreadSafe == "a7b339e4f6ff93a4697df5d7a87ff619" //power attack
                            || ability.Blueprint.AssetGuidThreadSafe == "94ed44fc6c8a717489eebdf8b364d4d8" //piranha strike
                            || ability.Blueprint.AssetGuidThreadSafe == "ccde5ab6edb84f346a74c17ea3e3a70c") //deadly aim
                        {
                            ability.TurnOffImmediately();
                        }
                    }
                }
            }
        }
    }
}
