using System;
using RedditImageScheduler.Utils;

namespace RedditImageScheduler.Data {
	public readonly struct ReddDateState {
		private readonly DateTimeOffset dateOffset;
		private readonly EReddDataStatus enumStatus;
		public ReddDateState(ReddDataEntry entry) {
			dateOffset = DateTimeOffset.FromUnixTimeSeconds(entry.Timestamp);
			if( entry.IsPosted ) {
				enumStatus = EReddDataStatus.POSTED;
			}
			else if( !entry.IsValid ) {
				enumStatus = EReddDataStatus.INVALID;
			}
			else {
				TimeSpan diff = dateOffset - DateTimeOffset.UtcNow;
				if( diff.Hours > 1 ) enumStatus = EReddDataStatus.VALID_HOURS;
				else if(diff.Minutes > 1 ) enumStatus = EReddDataStatus.VALID_MINUTES;
				else if(diff.Seconds >= 0 ) enumStatus = EReddDataStatus.VALID_SECONDS;
				else enumStatus = EReddDataStatus.VALID;
			}
		}
		
		public EReddDataStatus Status => enumStatus;
		public TimeSpan Time => dateOffset - DateTimeOffset.UtcNow;
	}
}