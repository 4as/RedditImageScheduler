using System;
using System.Collections.Generic;
using Eto.Drawing;

namespace RedditImageScheduler.Utils {
	public class ReddUtilCache<TKey> where TKey : IComparable<TKey> {
		private readonly Dictionary<TKey, LinkedListNode<CacheItem>> dictCache;
		private readonly LinkedList<CacheItem> listItems;
		private readonly uint nCapacity;

		public ReddUtilCache(uint limit) {
			nCapacity = limit;
			dictCache = new Dictionary<TKey, LinkedListNode<CacheItem>>((int)limit);
			listItems = new LinkedList<CacheItem>();
		}

		public uint Length => (uint)listItems.Count;

		public bool Has(TKey key) {
			return dictCache.ContainsKey(key);
		}

		public Bitmap Get(TKey key) {
			if( dictCache.TryGetValue(key, out var node) ) {
				listItems.Remove(node);
				listItems.AddFirst(node);
				return node.Value.Bitmap;
			}

			return null;
		}

		public void Add(TKey key, Bitmap bitmap) {
			if( dictCache.TryGetValue(key, out var target) ) {
				listItems.Remove(target);
				listItems.AddFirst(target);
				target.Value.Bitmap = bitmap;
			}
			else {
				if( dictCache.Count >= nCapacity ) {
					var last = listItems.Last;
					if( last != null ) {
						dictCache.Remove(last.Value.Key);
						listItems.RemoveLast();
						last.Value.Bitmap.Dispose();
					}
				}

				var newItem = new CacheItem(key, bitmap);
				var node = new LinkedListNode<CacheItem>(newItem);
				listItems.AddFirst(node);
				dictCache[key] = node;
			}
		}

		private class CacheItem {
			public TKey Key { get; }
			public Bitmap Bitmap { get; set; }

			public CacheItem(TKey key, Bitmap bitmap) {
				Key = key;
				Bitmap = bitmap;
			}
		}
	}
}