using System;
using System.ComponentModel;
using System.Configuration;

namespace Enyim.Caching.Configuration
{
	/// <summary>
	///		Summary descritpion for
	/// </summary>
	public class LoggerSection : ConfigurationSection
	{
		/// <summary>
		///
		/// </summary>
		[ConfigurationProperty("factory", IsRequired = true)]
		[InterfaceValidator(typeof(ILogFactory)), TypeConverter(typeof(TypeNameConverter))]
		public Type LogFactory
		{
			get { return (Type)base["factory"]; }
			set { base["factory"] = value; }
		}
	}
}
