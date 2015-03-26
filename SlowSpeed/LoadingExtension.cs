/*
 * Copyright 2015 SlowSpeed Developers
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ICities;
using UObject = UnityEngine.Object;

namespace SlowSpeed
{
	public class LoadingExtension : LoadingExtensionBase
	{
		readonly List<SlowVehicleInfo> vehicles = new List<SlowVehicleInfo>();
		readonly List<SlowCitizenInfo> citizens = new List<SlowCitizenInfo>();

		public override void OnLevelLoaded(LoadMode mode)
		{
			if (mode != LoadMode.NewGame && mode != LoadMode.LoadGame)
				return;

			ForEachPrefab((VehicleInfo i) =>
			{
				switch (i.m_vehicleType)
				{
					case VehicleInfo.VehicleType.Car:
						{
							var v = new SlowVehicleInfo(i);
							vehicles.Add(v);
							v.Apply(0.6f, 0.5f, 0.25f, 0.25f, 0.25f);
						}
						break;
					case VehicleInfo.VehicleType.Metro:
					case VehicleInfo.VehicleType.Train:
						{
							var v = new SlowVehicleInfo(i);
							vehicles.Add(v);
							v.Apply(0.5f);
						}
						break;
				}
			});
			ForEachPrefab((CitizenInfo i) =>
			{
				var c = new SlowCitizenInfo(i);
				citizens.Add(c);
				c.Apply(0.25f);
			});

			ForEachPrefab((VehicleInfo i) =>
			{
				ReplaceVehicleAI<PoliceCarAI, FastPoliceCarAI>(i);
				ReplaceVehicleAI<FireTruckAI, FastFireTruckAI>(i);
				ReplaceVehicleAI<AmbulanceAI, FastAmbulanceAI>(i);
			});
		}

		public override void OnLevelUnloading()
		{
			foreach (var v in vehicles)
				v.Restore();
			vehicles.Clear();
			foreach (var c in citizens)
				c.Restore();
			citizens.Clear();

			ForEachPrefab((VehicleInfo i) =>
			{
				ReplaceVehicleAI<FastPoliceCarAI, PoliceCarAI>(i);
				ReplaceVehicleAI<FastFireTruckAI, FireTruckAI>(i);
				ReplaceVehicleAI<FastAmbulanceAI, AmbulanceAI>(i);
			});
		}

		static void ReplaceVehicleAI<TOldAI, TNewAI>(VehicleInfo i)
			where TOldAI : VehicleAI where TNewAI : VehicleAI
		{
			// Requires the object to have the old AI
			var oldAI = i.gameObject.GetComponent<TOldAI>();
			if (oldAI == null || oldAI.GetType() != typeof(TOldAI))
				return;

			// Requires the object to not already have the new AI
			var newAI = i.gameObject.GetComponent<TNewAI>();
			if (newAI != null && newAI.GetType() == typeof(TNewAI))
			{
				CODebug.Log(LogChannel.Modding, string.Format("SlowSpeed: {0} already has {1}", i.name, typeof(TNewAI)));
				return;
			}

			CODebug.Log(LogChannel.Modding, string.Format("SlowSpeed: Replacing {0}'s {1} with {2}",
				i.name, typeof(TOldAI), typeof(TNewAI)));

			newAI = i.gameObject.AddComponent<TNewAI>();

			ShallowCopyTo(oldAI, newAI);

			oldAI.ReleaseAI();
			i.m_vehicleAI = newAI;
			UObject.Destroy(oldAI);
			newAI.InitializeAI();
		}

		static void ShallowCopyTo(object src, object dst)
		{
			var srcFields = GetFields(src);
			var dstFields = GetFields(dst);
			foreach (var srcField in srcFields)
			{
				FieldInfo dstField;

				if (!dstFields.TryGetValue(srcField.Key, out dstField))
					continue;

				CODebug.Log(LogChannel.Modding, string.Format("SlowSpeed: Setting {0} to {1} ({2} to {3})",
					dstField.Name, srcField.Value.GetValue(src), src.GetType(), dst.GetType()));

				dstField.SetValue(dst, srcField.Value.GetValue(src));
			}
		}

		static Dictionary<string, FieldInfo> GetFields(object obj)
		{
			return obj.GetType()
				.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
				.ToDictionary(f => f.Name, f => f);
		}

		static void ForEachPrefab<T>(Action<T> f) where T : PrefabInfo
		{
			for (var i = 0u; i < PrefabCollection<T>.LoadedCount(); i++)
				f(PrefabCollection<T>.GetLoaded(i));
		}
	}
}