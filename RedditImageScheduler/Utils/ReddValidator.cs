using System;
using System.Runtime.CompilerServices;
using RedditImageScheduler.Data;

namespace RedditImageScheduler.Utils {
	public static class ReddValidator {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsValidTitle(string title) {
			return !string.IsNullOrEmpty(title) && title.Length >= ReddConfig.ENTRY_TITLE_LENGTH;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsValidSource(string url) {
			return !string.IsNullOrEmpty(url) && Uri.TryCreate(url, UriKind.Absolute, out var result)
											  && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsValidDate(DateTime date) {
			return date >= DateTime.Now;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsValidImage(byte[] image_bytes) {
			return image_bytes != null && image_bytes.Length > 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsValid(ReddDataEntry entry) {
			return entry != null &&
				   IsValidTitle(entry.Title) &&
				   IsValidSource(entry.Source) &&
				   IsValidDate(entry.Date) &&
				   IsValidImage(entry.Image);
		}
	}
}