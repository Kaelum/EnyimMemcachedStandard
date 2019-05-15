using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;

using Enyim.Caching.Memcached;
using Enyim.Reflection;

namespace Enyim.Caching.Configuration
{
	/// <summary>
	/// This element is used to define locator/transcoder/keyTransformer instances. It also provides custom initializations for them using a factory.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public sealed class ProviderElement<T> : ConfigurationElement
		where T : class
	{
		// TODO make this element play nice with the configuration system (allow saving, etc.)
		private readonly Dictionary<string, string> _parameters = new Dictionary<string, string>();
		private IProviderFactory<T> _factoryInstance;

		/// <summary>
		/// Gets or sets the type of the provider.
		/// </summary>
		[ConfigurationProperty("type", IsRequired = false), TypeConverter(typeof(TypeNameConverter))]
		public Type Type
		{
			get { return (Type)base["type"]; }
			set
			{
				ConfigurationHelper.CheckForInterface(value, typeof(T));
				base["type"] = value;
			}
		}

		/// <summary>
		/// Gets or sets the type of the provider factory.
		/// </summary>
		[ConfigurationProperty("factory", IsRequired = false), TypeConverter(typeof(TypeNameConverter))]
		public Type Factory
		{
			get { return (Type)base["factory"]; }
			set
			{
				ConfigurationHelper.CheckForInterface(value, typeof(IProviderFactory<T>));

				base["factory"] = value;
			}
		}

		/// <summary>
		///		Summary description for
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
		{
			ConfigurationProperty property = new ConfigurationProperty(name, typeof(string), value);
			base[property] = value;

			_parameters[name] = value;

			return true;
		}

		/// <summary>
		/// Creates the provider by using the factory (if present) or directly instantiating by type name
		/// </summary>
		/// <returns></returns>
		public T CreateInstance()
		{
			//check if we have a factory
			if (_factoryInstance == null)
			{
				var type = Factory;
				if (type != null)
				{
					try
					{
						IProviderFactory<T> instance = (IProviderFactory<T>)FastActivator.Create(type);
						instance.Initialize(_parameters);

						_factoryInstance = instance;
					}
					catch (Exception e)
					{
						throw new InvalidOperationException(string.Format("Could not initialize the provider factory {0}. Check the InnerException for details.", type), e);
					}
				}
			}

			// no factory, use the provider type
			if (_factoryInstance == null)
			{
				var type = Type;

				if (type == null)
				{
					return null;
				}

				return (T)FastActivator.Create(type);
			}

			return _factoryInstance.Create();
		}

		/// <summary>
		///
		/// </summary>
		[ConfigurationProperty("data", IsRequired = false)]
		public TextElement Content
		{
			get { return (TextElement)base["data"]; }
			set { base["data"] = value; }
		}

		/// <summary>
		///
		/// </summary>
		protected override void PostDeserialize()
		{
			base.PostDeserialize();

			var c = Content;
			if (c != null)
			{
				_parameters[string.Empty] = c.Content;
			}
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
