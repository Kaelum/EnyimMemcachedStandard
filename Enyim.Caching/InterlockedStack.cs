﻿using System;
using System.Threading;

namespace Enyim.Collections
{
	/// <summary>
	/// Implements a non-locking stack.
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
	public class InterlockedStack<TItem>
	{
		private Node _head;

		/// <summary>
		///
		/// </summary>
		public InterlockedStack()
		{
			_head = new Node(default);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="item"></param>
		public void Push(TItem item)
		{
			Node node = new Node(item);

			do { node.Next = _head.Next; }
			while (Interlocked.CompareExchange(ref _head.Next, node, node.Next) != node.Next);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool TryPop(out TItem value)
		{
			value = default;
			Node node;

			do
			{
				node = _head.Next;
				if (node == null)
				{
					return false;
				}
			}
			while (Interlocked.CompareExchange(ref _head.Next, node.Next, node) != node);

			value = node.Value;

			return true;
		}

		#region [ Node                        ]

		private class Node
		{
			public readonly TItem Value;
			public Node Next;

			/// <summary>
			///
			/// </summary>
			/// <param name="value"></param>
			public Node(TItem value)
			{
				Value = value;
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
