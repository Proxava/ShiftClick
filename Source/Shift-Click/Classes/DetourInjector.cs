using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using RimWorld;
using Verse;

//
//
// Source of Detourmethods: https://github.com/Zhentar/BetterPathfinding/blob/bpmx/BetterPathfinding/RegionPathCostHeuristic.cs
// Credits go to Zhentar and RawCode
//
//

namespace ShiftClick
{
    [StaticConstructorOnStartup]

    internal static class DetourInjector
    {
        private static Assembly Assembly => Assembly.GetAssembly(typeof(DetourInjector));

        private static string AssemblyName => Assembly.FullName.Split(',').First();

        private static bool DoInject()
        {
            String targetMethod = "DoConfigInterface";
            String replaceMethod = "DoConfigInterface";

            MethodInfo RimWorld_Bill_Production_Draw = typeof(RimWorld.Bill_Production).GetMethod(targetMethod, UniversalBindingFlags); // new [] { typeof(Rect), typeof(Color) }
            MethodInfo ModTest_Bill_Production_Draw = typeof(Bill_ProductionMod).GetMethod(replaceMethod, UniversalBindingFlags);

            if (RimWorld_Bill_Production_Draw == null)
                Log.Error("Fuck");

            if (!Detours.TryDetourFromTo(RimWorld_Bill_Production_Draw, ModTest_Bill_Production_Draw))
                return false;

            return true;
        }

        static DetourInjector()
        {
            LongEventHandler.QueueLongEvent(Inject, "Initializing", true, null);

#if DEBUG

			if (Prefs.DevMode)
			{
				DebugViewSettings.drawPaths = true;
			}

#endif

        }

        private static void Inject()
        {
            if (DoInject())
                Log.Message(AssemblyName + " injected.");
            else
                Log.Error(AssemblyName + " failed to get injected properly.");
        }

        private const BindingFlags UniversalBindingFlags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private static bool DoDetour(Type rimworld, Type mod, string method)
        {
            MethodInfo RimWorld_A = rimworld.GetMethod(method, UniversalBindingFlags);
            MethodInfo ModTest_A = mod.GetMethod(method, UniversalBindingFlags);
            if (!Detours.TryDetourFromTo(RimWorld_A, ModTest_A))
                return false;
            return true;
        }

    }
}

