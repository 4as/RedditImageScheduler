using System.Diagnostics;
using System.Runtime.InteropServices;
using Reddit.AuthTokenRetriever.EventArgs;
using RedditImageScheduler.IO.Web;

namespace RedditImageScheduler.IO {
	public class ReddIOWeb {
		private readonly ReddIOAuth reddAuth = new ReddIOAuth(); 

		// ===============================================
		// GETTERS / SETTERS
		public bool IsListening => reddAuth.IsListening;
		public string RedirectUrl => reddAuth.RedirectUrl;
		public string AccessToken => reddAuth.AccessToken;
		public string RefreshToken => reddAuth.RefreshToken;

		// ===============================================
		// PUBLIC METHODS
		public void Start(string auth_id, string auth_secret) {
			if( string.IsNullOrEmpty(auth_id) ) {
				OnError();
				return;
			}

			Stop();

			reddAuth.EventSuccess += OnAuthorization;
			reddAuth.EventFailure += OnError;
			reddAuth.Request(auth_id, auth_secret);
		}

		public void Stop() {
			reddAuth.EventSuccess -= OnAuthorization;
			reddAuth.EventFailure -= OnError;
			reddAuth.Close();
		}
		
		// ===============================================
		// EVENTS
		public delegate void IOWebEvent();
		public event IOWebEvent EventSuccess;
		public event IOWebEvent EventFailure;

		// ===============================================
		// CALLBACKS
		private void OnError() {
			Stop();
			EventFailure?.Invoke();
		}
		
		private void OnAuthorization(string access_token, string refresh_token) {
			if( string.IsNullOrEmpty(access_token) || string.IsNullOrEmpty(refresh_token) ) {
				OnError();
			}
			else {
				Stop();
				EventSuccess?.Invoke();
			}
		}

		// ===============================================
		// HELPERS
		public static Process OpenBrowser(string authUrl = "about:blank")
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				return Process.Start(new ProcessStartInfo(authUrl) { UseShellExecute = true });
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				return Process.Start("open", authUrl);
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				return Process.Start("xdg-open", authUrl);
			}

			return null;
		}
	}
}