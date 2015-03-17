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
		readonly List<SlowVehicleInfo> vehicles = new List<SlowVehicleInfo>();
		readonly List<SlowCitizenInfo> citizens = new List<SlowCitizenInfo>();

		public override void OnLevelLoaded(LoadMode mode)
		{
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

		static void ForEachPrefab<T>(Action<T> f) where T : PrefabInfo
		{
			for (var i = 0u; i < PrefabCollection<T>.PrefabCount(); i++)
				f(PrefabCollection<T>.GetPrefab(i));
		}
	}

	class SlowVehicleInfo
	{
		readonly VehicleInfo i;
		public readonly float BaseMaxSpeed;
		public readonly float BaseLeanMultiplier;
		public readonly float BaseAcceleration;
		public readonly float BaseBraking;
		public readonly float BaseTurning;

		public SlowVehicleInfo(VehicleInfo i)
		{
			this.i = i;
			BaseMaxSpeed = i.m_maxSpeed;
			BaseLeanMultiplier = i.m_leanMultiplier;
			BaseAcceleration = i.m_acceleration;
			BaseBraking = i.m_braking;
			BaseTurning = i.m_turning;
		}

		public void Apply(float maxSpeedMultiplier, float leanMultiplierMultiplier, float accelerationMultiplier,
			float brakingMultiplier, float turningMultiplier)
		{
			i.m_maxSpeed = BaseMaxSpeed * maxSpeedMultiplier;
			i.m_leanMultiplier = BaseLeanMultiplier * leanMultiplierMultiplier;
			i.m_acceleration = BaseAcceleration * accelerationMultiplier;
			i.m_braking = BaseBraking * brakingMultiplier;
			i.m_turning = BaseTurning * turningMultiplier;
		}

		public void Apply(float maxSpeedMultiplier)
		{
			i.m_maxSpeed = BaseMaxSpeed * maxSpeedMultiplier;
		}

		public void Restore()
		{
			i.m_maxSpeed = BaseMaxSpeed;
			i.m_leanMultiplier = BaseLeanMultiplier;
			i.m_acceleration = BaseAcceleration;
			i.m_braking = BaseBraking;
			i.m_turning = BaseTurning;
		}
	}

	class SlowCitizenInfo
	{
		readonly CitizenInfo i;
		public readonly float BaseWalkSpeed;

		public SlowCitizenInfo(CitizenInfo i)
		{
			this.i = i;
			BaseWalkSpeed = i.m_walkSpeed;
		}

		public void Apply(float walkSpeedMultiplier)
		{
			i.m_walkSpeed = BaseWalkSpeed * walkSpeedMultiplier;
		}

		public void Restore()
		{
			i.m_walkSpeed = BaseWalkSpeed;
		}
	}
}
