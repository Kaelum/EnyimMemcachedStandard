using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Enyim.Caching.Memcached
{
	/// <summary>
	/// Default <see cref="T:Enyim.Caching.Memcached.ITranscoder"/> implementation. Primitive types are manually serialized, the rest is serialized using <see cref="T:System.Runtime.Serialization.Formatters.Binary.BinaryFormatter"/>.
	/// </summary>
	public class DefaultTranscoder : ITranscoder
	{
		/// <summary></summary>
		public const uint RawDataFlag = 0xfa52;

		private static readonly ArraySegment<byte> _nullArray = new ArraySegment<byte>(new byte[0]);

		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		CacheItem ITranscoder.Serialize(object value)
		{
			return Serialize(value);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		object ITranscoder.Deserialize(CacheItem item)
		{
			return Deserialize(item);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected virtual CacheItem Serialize(object value)
		{
			// raw data is a special case when some1 passes in a buffer (byte[] or ArraySegment<byte>)
			if (value is ArraySegment<byte>)
			{
				// ArraySegment<byte> is only passed in when a part of buffer is being
				// serialized, usually from a MemoryStream (To avoid duplicating arrays
				// the byte[] returned by MemoryStream.GetBuffer is placed into an ArraySegment.)
				return new CacheItem(RawDataFlag, (ArraySegment<byte>)value);
			}


			// - or we just received a byte[]. No further processing is needed.
			if (value is byte[] tmpByteArray)
			{
				return new CacheItem(RawDataFlag, new ArraySegment<byte>(tmpByteArray));
			}

			ArraySegment<byte> data;
			TypeCode code = value == null ? TypeCode.DBNull : Type.GetTypeCode(value.GetType());

			switch (code)
			{
				case TypeCode.DBNull: data = SerializeNull(); break;
				case TypeCode.String: data = SerializeString((string)value); break;
				case TypeCode.Boolean: data = SerializeBoolean((bool)value); break;
				case TypeCode.SByte: data = SerializeSByte((sbyte)value); break;
				case TypeCode.Byte: data = SerializeByte((byte)value); break;
				case TypeCode.Int16: data = SerializeInt16((short)value); break;
				case TypeCode.Int32: data = SerializeInt32((int)value); break;
				case TypeCode.Int64: data = SerializeInt64((long)value); break;
				case TypeCode.UInt16: data = SerializeUInt16((ushort)value); break;
				case TypeCode.UInt32: data = SerializeUInt32((uint)value); break;
				case TypeCode.UInt64: data = SerializeUInt64((ulong)value); break;
				case TypeCode.Char: data = SerializeChar((char)value); break;
				case TypeCode.DateTime: data = SerializeDateTime((DateTime)value); break;
				case TypeCode.Double: data = SerializeDouble((double)value); break;
				case TypeCode.Single: data = SerializeSingle((float)value); break;
				default:
					code = TypeCode.Object;
					data = SerializeObject(value);
					break;
			}

			return new CacheItem(TypeCodeToFlag(code), data);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public static uint TypeCodeToFlag(TypeCode code)
		{
			return (uint)((int)code | 0x0100);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="flag"></param>
		/// <returns></returns>
		public static bool IsFlagHandled(uint flag)
		{
			return (flag & 0x100) == 0x100;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		protected virtual object Deserialize(CacheItem item)
		{
			if (item.Data.Array == null)
			{
				return null;
			}

			if (item.Flags == RawDataFlag)
			{
				var tmp = item.Data;

				if (tmp.Count == tmp.Array.Length)
				{
					return tmp.Array;
				}

				// we should never arrive here, but it's better to be safe than sorry
				byte[] retval = new byte[tmp.Count];

				Array.Copy(tmp.Array, tmp.Offset, retval, 0, tmp.Count);

				return retval;
			}

			TypeCode code = (TypeCode)(item.Flags & 0xff);

			var data = item.Data;

			switch (code)
			{
				// incrementing a non-existing key then getting it
				// returns as a string, but the flag will be 0
				// so treat all 0 flagged items as string
				// this may help inter-client data management as well
				//
				// however we store 'null' as Empty + an empty array,
				// so this must special-cased for compatibilty with
				// earlier versions. we introduced DBNull as null marker in emc2.6
				case TypeCode.Empty:
				{
					return (data.Array == null || data.Count == 0)
							? null
							: DeserializeString(data);
				}

				case TypeCode.DBNull:
				{
					return null;
				}

				case TypeCode.String:
				{
					return DeserializeString(data);
				}

				case TypeCode.Boolean:
				{
					return DeserializeBoolean(data);
				}

				case TypeCode.Int16:
				{
					return DeserializeInt16(data);
				}

				case TypeCode.Int32:
				{
					return DeserializeInt32(data);
				}

				case TypeCode.Int64:
				{
					return DeserializeInt64(data);
				}

				case TypeCode.UInt16:
				{
					return DeserializeUInt16(data);
				}

				case TypeCode.UInt32:
				{
					return DeserializeUInt32(data);
				}

				case TypeCode.UInt64:
				{
					return DeserializeUInt64(data);
				}

				case TypeCode.Char:
				{
					return DeserializeChar(data);
				}

				case TypeCode.DateTime:
				{
					return DeserializeDateTime(data);
				}

				case TypeCode.Double:
				{
					return DeserializeDouble(data);
				}

				case TypeCode.Single:
				{
					return DeserializeSingle(data);
				}

				case TypeCode.Byte:
				{
					return DeserializeByte(data);
				}

				case TypeCode.SByte:
				{
					return DeserializeSByte(data);
				}

				// backward compatibility
				// earlier versions serialized decimals with TypeCode.Decimal
				// even though they were saved by BinaryFormatter
				case TypeCode.Decimal:
				case TypeCode.Object:
				{
					return DeserializeObject(data);
				}

				default:
				{
					throw new InvalidOperationException("Unknown TypeCode was returned: " + code);
				}
			}
		}

		#region [ Typed serialization          ]

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		protected virtual ArraySegment<byte> SerializeNull()
		{
			return _nullArray;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected virtual ArraySegment<byte> SerializeString(string value)
		{
			return new ArraySegment<byte>(Encoding.UTF8.GetBytes((string)value));
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected virtual ArraySegment<byte> SerializeByte(byte value)
		{
			return new ArraySegment<byte>(new byte[] { value });
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected virtual ArraySegment<byte> SerializeSByte(sbyte value)
		{
			return new ArraySegment<byte>(new byte[] { (byte)value });
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected virtual ArraySegment<byte> SerializeBoolean(bool value)
		{
			return new ArraySegment<byte>(BitConverter.GetBytes(value));
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected virtual ArraySegment<byte> SerializeInt16(short value)
		{
			return new ArraySegment<byte>(BitConverter.GetBytes(value));
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected virtual ArraySegment<byte> SerializeInt32(int value)
		{
			return new ArraySegment<byte>(BitConverter.GetBytes(value));
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected virtual ArraySegment<byte> SerializeInt64(long value)
		{
			return new ArraySegment<byte>(BitConverter.GetBytes(value));
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected virtual ArraySegment<byte> SerializeUInt16(ushort value)
		{
			return new ArraySegment<byte>(BitConverter.GetBytes(value));
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected virtual ArraySegment<byte> SerializeUInt32(uint value)
		{
			return new ArraySegment<byte>(BitConverter.GetBytes(value));
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected virtual ArraySegment<byte> SerializeUInt64(ulong value)
		{
			return new ArraySegment<byte>(BitConverter.GetBytes(value));
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected virtual ArraySegment<byte> SerializeChar(char value)
		{
			return new ArraySegment<byte>(BitConverter.GetBytes(value));
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected virtual ArraySegment<byte> SerializeDateTime(DateTime value)
		{
			return new ArraySegment<byte>(BitConverter.GetBytes(value.ToBinary()));
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected virtual ArraySegment<byte> SerializeDouble(double value)
		{
			return new ArraySegment<byte>(BitConverter.GetBytes(value));
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected virtual ArraySegment<byte> SerializeSingle(float value)
		{
			return new ArraySegment<byte>(BitConverter.GetBytes(value));
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected virtual ArraySegment<byte> SerializeObject(object value)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				new BinaryFormatter().Serialize(ms, value);

				return new ArraySegment<byte>(ms.GetBuffer(), 0, (int)ms.Length);
			}
		}

		#endregion

		#region [ Typed deserialization        ]

		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected virtual string DeserializeString(ArraySegment<byte> value)
		{
			return Encoding.UTF8.GetString(value.Array, value.Offset, value.Count);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected virtual bool DeserializeBoolean(ArraySegment<byte> value)
		{
			return BitConverter.ToBoolean(value.Array, value.Offset);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected virtual short DeserializeInt16(ArraySegment<byte> value)
		{
			return BitConverter.ToInt16(value.Array, value.Offset);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected virtual int DeserializeInt32(ArraySegment<byte> value)
		{
			return BitConverter.ToInt32(value.Array, value.Offset);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected virtual long DeserializeInt64(ArraySegment<byte> value)
		{
			return BitConverter.ToInt64(value.Array, value.Offset);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected virtual ushort DeserializeUInt16(ArraySegment<byte> value)
		{
			return BitConverter.ToUInt16(value.Array, value.Offset);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected virtual uint DeserializeUInt32(ArraySegment<byte> value)
		{
			return BitConverter.ToUInt32(value.Array, value.Offset);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected virtual ulong DeserializeUInt64(ArraySegment<byte> value)
		{
			return BitConverter.ToUInt64(value.Array, value.Offset);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected virtual char DeserializeChar(ArraySegment<byte> value)
		{
			return BitConverter.ToChar(value.Array, value.Offset);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected virtual DateTime DeserializeDateTime(ArraySegment<byte> value)
		{
			return DateTime.FromBinary(BitConverter.ToInt64(value.Array, value.Offset));
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected virtual double DeserializeDouble(ArraySegment<byte> value)
		{
			return BitConverter.ToDouble(value.Array, value.Offset);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected virtual float DeserializeSingle(ArraySegment<byte> value)
		{
			return BitConverter.ToSingle(value.Array, value.Offset);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		protected virtual byte DeserializeByte(ArraySegment<byte> data)
		{
			return data.Array[data.Offset];
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		protected virtual sbyte DeserializeSByte(ArraySegment<byte> data)
		{
			return (sbyte)data.Array[data.Offset];
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected virtual object DeserializeObject(ArraySegment<byte> value)
		{
			using (MemoryStream ms = new MemoryStream(value.Array, value.Offset, value.Count))
			{
				BinaryFormatter formatter = new BinaryFormatter();
				//formatter.Binder = new MemcachedSerializationBinder();

				return formatter.Deserialize(ms);
			}
		}

		#endregion
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
