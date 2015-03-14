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
using ICities;

namespace SlowSpeed
{
	public class Mod : IUserMod
	{
		public string Description
		{
			get { return "Slows down vehicles and pedestrians"; }
		}

		public string Name
		{
			get { return "Slow Speed"; }
		}
	}

	public class LoadingExtension : LoadingExtensionBase
	{
		public override void OnLevelLoaded(LoadMode mode)
		{
			ForEachPrefab((VehicleInfo i) =>
			{
				switch (i.m_vehicleType)
				{
					case VehicleInfo.VehicleType.Car:
						i.m_maxSpeed *= 0.6f;
						i.m_leanMultiplier *= 0.5f;
						i.m_acceleration *= 0.25f;
						i.m_braking *= 0.25f;
						i.m_turning *= 0.25f;
						break;
					case VehicleInfo.VehicleType.Metro:
					case VehicleInfo.VehicleType.Train:
						i.m_maxSpeed *= 0.5f;
						break;
				}
			});
			ForEachPrefab((CitizenInfo i) => i.m_walkSpeed *= 0.25f);
		}

		static void ForEachPrefab<T>(Action<T> f) where T : PrefabInfo
		{
			for (var i = 0u; i < PrefabCollection<T>.PrefabCount(); i++)
				f(PrefabCollection<T>.GetPrefab(i));
		}
	}
}
