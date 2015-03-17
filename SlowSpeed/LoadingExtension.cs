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
			ForEachPrefab((VehicleInfo i) =>
			{
				ReplaceVehicleAI<PoliceCarAI, FastPoliceCarVehicleAI>(i);
				ReplaceVehicleAI<FireTruckAI, FastFireTruckVehicleAI>(i);
				ReplaceVehicleAI<AmbulanceAI, FastAmbulanceVehicleAI>(i);
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
		}

		public override void OnLevelUnloading()
		{
			foreach (var v in vehicles)
				v.Restore();
			vehicles.Clear();
			foreach (var c in citizens)
				c.Restore();
			citizens.Clear();
		}

		static void ReplaceVehicleAI<TOldAI, TNewAI>(VehicleInfo i) where TOldAI : VehicleAI where TNewAI : VehicleAI, IVehicleAIReplacement<TOldAI>, new()
		{
			var oldAI = i.GetComponent<TOldAI>();
			if (oldAI == null)
				return;
			var newAI = i.gameObject.AddComponent<TNewAI>();
			newAI.ReplaceVehicleAI(i, oldAI);
			UObject.Destroy(oldAI);
			newAI.InitializeAI();
		}

		static void ForEachPrefab<T>(Action<T> f) where T : PrefabInfo
		{
			for (var i = 0u; i < PrefabCollection<T>.PrefabCount(); i++)
				f(PrefabCollection<T>.GetPrefab(i));
		}
	}
}
