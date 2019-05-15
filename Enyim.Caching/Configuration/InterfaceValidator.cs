using System;
using System.Configuration;

namespace Enyim.Caching.Configuration
{
	/// <summary>
	///		Summary description for
	/// </summary>
	public class InterfaceValidator : ConfigurationValidatorBase
	{
		private readonly Type _interfaceType;

		/// <summary>
		///
		/// </summary>
		/// <param name="type"></param>
		public InterfaceValidator(Type type)
		{
			if (!type.IsInterface)
			{
				throw new ArgumentException(type + " must be an interface");
			}

			_interfaceType = type;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public override bool CanValidate(Type type)
		{
			return (type == typeof(Type)) || base.CanValidate(type);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		public override void Validate(object value)
		{
			if (value != null)
			{
				ConfigurationHelper.CheckForInterface((Type)value, _interfaceType);
			}
		}
	}

	/// <summary>
	///		Summary description for
	/// </summary>
	public sealed class InterfaceValidatorAttribute : ConfigurationValidatorAttribute
	{
		private readonly Type _interfaceType;

		/// <summary>
		///
		/// </summary>
		/// <param name="type"></param>
		public InterfaceValidatorAttribute(Type type)
		{
			if (!type.IsInterface)
			{
				throw new ArgumentException(type + " must be an interface");
			}

			_interfaceType = type;
		}

		/// <summary>
		///
		/// </summary>
		public override ConfigurationValidatorBase ValidatorInstance
		{
			get { return new InterfaceValidator(_interfaceType); }
		}
	}
}

#region [ License information          ]
/* ************************************************************
 *
 *    Copyright (c) 2010 Attila Kiskó, enyim.com
 *
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * ************************************************************/
#endregion
