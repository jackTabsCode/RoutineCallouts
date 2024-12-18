﻿using System;
using System.Reflection;
using LSPD_First_Response.Mod.API;

namespace RoutineCallouts
{
	public class Main : Plugin
	{
		public override void Initialize()
		{
			Functions.OnOnDutyStateChanged += OnDutyChangedHandler;
			AppDomain.CurrentDomain.AssemblyResolve += ResolveEventHandler;

			Logger.Log("Initialized");
		}

		private void OnDutyChangedHandler(bool onDuty)
		{
			if (!onDuty) return;

			RegisterCallouts();
		}

		public override void Finally()
		{
			Logger.Log("Cleaned up");
		}

		private static void RegisterCallouts()
		{
			Functions.RegisterCallout(typeof(Callouts.HighSpeedChase));
		}

		private static Assembly ResolveEventHandler(object sender, ResolveEventArgs args)
		{
			foreach (Assembly assembly in Functions.GetAllUserPlugins())
			{
				AssemblyName assemblyName = assembly.GetName();

				if (args.Name != null && assemblyName.Name != null &&
					args.Name.ToLower().Contains(assemblyName.Name.ToLower()))
				{
					return assembly;
				}
			}

			return null;
		}

		public static bool IsRoutineCalloutsRunning(string plugin, Version minVersion = null)
		{
			foreach (Assembly assembly in Functions.GetAllUserPlugins())
			{
				AssemblyName assemblyName = assembly.GetName();

				if (assemblyName.Name != null &&
					string.Equals(assemblyName.Name, plugin, StringComparison.OrdinalIgnoreCase))
				{
					if (minVersion == null || (assemblyName.Version != null && assemblyName.Version.CompareTo(minVersion) >= 0))
					{
						return true;
					}
				}
			}

			return false;
		}
	}
}
