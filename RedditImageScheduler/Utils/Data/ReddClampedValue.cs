using System;

namespace RedditImageScheduler.Utils.Data {
	public class ReddClampedValue<TValue> where TValue : struct, IComparable<TValue> {
		private readonly ReddClamping<TValue> reddClamping;
		private TValue nValue;

		public ReddClampedValue(TValue min, TValue max, TValue value = default) {
			reddClamping = new ReddClamping<TValue>(min, max);
			nValue = reddClamping.Clamp(value);
		}

		public ReddClampedValue(ReddClamping<TValue> clamping, TValue value = default) {
			reddClamping = clamping;
			nValue = reddClamping.Clamp(value);
		}

		public TValue Value {
			get => nValue;
			set => nValue = reddClamping.Clamp(value);
		}
	}
}