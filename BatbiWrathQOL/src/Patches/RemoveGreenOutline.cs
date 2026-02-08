using HarmonyLib;
using Kingmaker.PubSubSystem;
using Kingmaker.View;
using Kingmaker.Visual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BatbiWrathQOL.Patches
{
    //**********************************
    // Remove green outline
    //**********************************
    [HarmonyPatch(typeof(UnitEntityView), nameof(UnitEntityView.UpdateHighlight))]
    internal static class UnitEntityView_UpdateHighlight_Patch
    {
        static bool Prefix(UnitEntityView __instance, ref UnitMultiHighlight ___m_Highlighter, bool raiseEvent = true)
        {
            if (__instance.EntityData == null) return true;

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
}
