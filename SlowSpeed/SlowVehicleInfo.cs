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

namespace SlowSpeed
{
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
}
