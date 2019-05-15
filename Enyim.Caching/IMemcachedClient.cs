using System;
using System.Collections.Generic;

using Enyim.Caching.Memcached;

namespace Enyim.Caching
{
	/// <summary>
	///		Summary description for
	/// </summary>
	public interface IMemcachedClient : IDisposable
	{
		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		object Get(string key);

		/// <summary>
		///
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <returns></returns>
		T Get<T>(string key);

		/// <summary>
		///
		/// </summary>
		/// <param name="keys"></param>
		/// <returns></returns>
		IDictionary<string, object> Get(IEnumerable<string> keys);

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		bool TryGet(string key, out object value);

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		bool TryGetWithCas(string key, out CasResult<object> value);

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		CasResult<object> GetWithCas(string key);

		/// <summary>
		///
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <returns></returns>
		CasResult<T> GetWithCas<T>(string key);

		/// <summary>
		///
		/// </summary>
		/// <param name="keys"></param>
		/// <returns></returns>
		IDictionary<string, CasResult<object>> GetWithCas(IEnumerable<string> keys);

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		bool Append(string key, ArraySegment<byte> data);

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <param name="cas"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		CasResult<bool> Append(string key, ulong cas, ArraySegment<byte> data);

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		bool Prepend(string key, ArraySegment<byte> data);

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <param name="cas"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		CasResult<bool> Prepend(string key, ulong cas, ArraySegment<byte> data);

		/// <summary>
		///
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		bool Store(StoreMode mode, string key, object value);

		/// <summary>
		///
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="expiresAt"></param>
		/// <returns></returns>
		bool Store(StoreMode mode, string key, object value, DateTime expiresAt);

		/// <summary>
		///
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="validFor"></param>
		/// <returns></returns>
		bool Store(StoreMode mode, string key, object value, TimeSpan validFor);

		/// <summary>
		///
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		CasResult<bool> Cas(StoreMode mode, string key, object value);

		/// <summary>
		///
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="cas"></param>
		/// <returns></returns>
		CasResult<bool> Cas(StoreMode mode, string key, object value, ulong cas);

		/// <summary>
		///
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="expiresAt"></param>
		/// <param name="cas"></param>
		/// <returns></returns>
		CasResult<bool> Cas(StoreMode mode, string key, object value, DateTime expiresAt, ulong cas);

		/// <summary>
		///
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="validFor"></param>
		/// <param name="cas"></param>
		/// <returns></returns>
		CasResult<bool> Cas(StoreMode mode, string key, object value, TimeSpan validFor, ulong cas);

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <param name="delta"></param>
		/// <returns></returns>
		ulong Decrement(string key, ulong defaultValue, ulong delta);

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <param name="delta"></param>
		/// <param name="expiresAt"></param>
		/// <returns></returns>
		ulong Decrement(string key, ulong defaultValue, ulong delta, DateTime expiresAt);

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <param name="delta"></param>
		/// <param name="validFor"></param>
		/// <returns></returns>
		ulong Decrement(string key, ulong defaultValue, ulong delta, TimeSpan validFor);

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <param name="delta"></param>
		/// <param name="cas"></param>
		/// <returns></returns>
		CasResult<ulong> Decrement(string key, ulong defaultValue, ulong delta, ulong cas);

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <param name="delta"></param>
		/// <param name="expiresAt"></param>
		/// <param name="cas"></param>
		/// <returns></returns>
		CasResult<ulong> Decrement(string key, ulong defaultValue, ulong delta, DateTime expiresAt, ulong cas);

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <param name="delta"></param>
		/// <param name="validFor"></param>
		/// <param name="cas"></param>
		/// <returns></returns>
		CasResult<ulong> Decrement(string key, ulong defaultValue, ulong delta, TimeSpan validFor, ulong cas);

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <param name="delta"></param>
		/// <returns></returns>
		ulong Increment(string key, ulong defaultValue, ulong delta);

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <param name="delta"></param>
		/// <param name="expiresAt"></param>
		/// <returns></returns>
		ulong Increment(string key, ulong defaultValue, ulong delta, DateTime expiresAt);

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <param name="delta"></param>
		/// <param name="validFor"></param>
		/// <returns></returns>
		ulong Increment(string key, ulong defaultValue, ulong delta, TimeSpan validFor);

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <param name="delta"></param>
		/// <param name="cas"></param>
		/// <returns></returns>
		CasResult<ulong> Increment(string key, ulong defaultValue, ulong delta, ulong cas);

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <param name="delta"></param>
		/// <param name="expiresAt"></param>
		/// <param name="cas"></param>
		/// <returns></returns>
		CasResult<ulong> Increment(string key, ulong defaultValue, ulong delta, DateTime expiresAt, ulong cas);

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <param name="delta"></param>
		/// <param name="validFor"></param>
		/// <param name="cas"></param>
		/// <returns></returns>
		CasResult<ulong> Increment(string key, ulong defaultValue, ulong delta, TimeSpan validFor, ulong cas);

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		bool Remove(string key);

		/// <summary>
		///
		/// </summary>
		void FlushAll();

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		ServerStats Stats();

		/// <summary>
		///
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		ServerStats Stats(string type);

		/// <summary></summary>
		event Action<IMemcachedNode> NodeFailed;
	}
}
