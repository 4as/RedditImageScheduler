using System;

namespace RedditImageScheduler.Utils.Data {
	public readonly struct ReddClamping<TValue> where TValue : struct, IComparable<TValue> {
		public readonly TValue Min;
		public readonly TValue Max;
		public ReddClamping(TValue min, TValue max) {
			Min = min;
			Max = max;
		}
		public TValue Clamp(TValue value) => value.Clamp(Min, Max);
	}
}