
namespace Enyim.Caching.Memcached.Protocol.Binary
{
	/// <summary>
	///		Summary description for
	/// </summary>
	public enum OpCode : byte
	{
		/// <summary></summary>
		Get = 0x00,

		/// <summary></summary>
		Set = 0x01,

		/// <summary></summary>
		Add = 0x02,

		/// <summary></summary>
		Replace = 0x03,

		/// <summary></summary>
		Delete = 0x04,

		/// <summary></summary>
		Increment = 0x05,

		/// <summary></summary>
		Decrement = 0x06,

		/// <summary></summary>
		Quit = 0x07,

		/// <summary></summary>
		Flush = 0x08,

		/// <summary></summary>
		GetQ = 0x09,

		/// <summary></summary>
		NoOp = 0x0A,

		/// <summary></summary>
		Version = 0x0B,

		/// <summary></summary>
		GetK = 0x0C,

		/// <summary></summary>
		GetKQ = 0x0D,

		/// <summary></summary>
		Append = 0x0E,

		/// <summary></summary>
		Prepend = 0x0F,

		/// <summary></summary>
		Stat = 0x10,

		/// <summary></summary>
		SetQ = 0x11,

		/// <summary></summary>
		AddQ = 0x12,

		/// <summary></summary>
		ReplaceQ = 0x13,

		/// <summary></summary>
		DeleteQ = 0x14,

		/// <summary></summary>
		IncrementQ = 0x15,

		/// <summary></summary>
		DecrementQ = 0x16,

		/// <summary></summary>
		QuitQ = 0x17,

		/// <summary></summary>
		FlushQ = 0x18,

		/// <summary></summary>
		AppendQ = 0x19,

		/// <summary></summary>
		PrependQ = 0x1A,

		// SASL authentication op-codes
		/// <summary></summary>
		SaslList = 0x20,

		/// <summary></summary>
		SaslStart = 0x21,

		/// <summary></summary>
		SaslStep = 0x22
	};
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
