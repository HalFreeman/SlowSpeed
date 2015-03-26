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

using UnityEngine;

namespace SlowSpeed
{
	class FastAmbulanceAI : AmbulanceAI
	{
		protected override float CalculateTargetSpeed(ushort vehicleID, ref Vehicle data, float speedLimit, float curve)
		{
			if ((data.m_flags & Vehicle.Flags.Emergency2) == Vehicle.Flags.None)
				return base.CalculateTargetSpeed(vehicleID, ref data, speedLimit, curve);

			return Mathf.Min(base.CalculateTargetSpeed(vehicleID, ref data, speedLimit * 2, curve * 0.5f), m_info.m_maxSpeed * 2);
		}

		// HACK
		public override void SimulationStep(ushort vehicleID, ref Vehicle vehicleData, ref Vehicle.Frame frameData, ushort leaderID,
			ref Vehicle leaderData, int lodPhysics)
		{
			var baseAcceleration = m_info.m_acceleration;

			if ((vehicleData.m_flags & Vehicle.Flags.Emergency2) == Vehicle.Flags.Emergency2)
				m_info.m_acceleration *= 3;

			base.SimulationStep(vehicleID, ref vehicleData, ref frameData, leaderID, ref leaderData, lodPhysics);

			m_info.m_acceleration = baseAcceleration;
		}
	}
}
