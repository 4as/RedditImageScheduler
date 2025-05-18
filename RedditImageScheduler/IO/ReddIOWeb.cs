using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;
using Reddit.AuthTokenRetriever;
using Reddit.AuthTokenRetriever.EventArgs;
using Timer = System.Timers.Timer;

namespace RedditImageScheduler.IO {
	public class ReddIOWeb {
		private readonly Timer sysTimer = new Timer(5000);
		
		private SynchronizationContext sysContext = SynchronizationContext.Current;
		private AuthTokenRetrieverLib authRetriever;

		private string sAccessToken;
		private string sRefreshToken;

		// ===============================================
		// GETTERS / SETTERS
		public bool IsListening => authRetriever != null;
		public string AccessToken => sAccessToken;
		public string RefreshToken => sRefreshToken;

		// ===============================================
		// PUBLIC METHODS
		public void Start(string auth_id, string auth_secret) {
			Stop();
			
			if( sysContext == null ) sysContext = SynchronizationContext.Current;
			
			authRetriever = new AuthTokenRetrieverLib(auth_id, 8080, "localhost", null, auth_secret);
			authRetriever.AuthSuccess += OnAuthorization;
			try {
				StartTimer();
				//TODO: needs to be rebuilt, since socket remains open, even after calling StopListening()
				authRetriever.AwaitCallback();
				OpenBrowser(authRetriever.AuthURL());
			}
			catch( Exception ) {
				OnError();
			}
		}

		public void Stop() {
			if( authRetriever != null ) {
				authRetriever.AuthSuccess -= OnAuthorization;
				authRetriever.StopListening();
				authRetriever = null;
			}

			StopTimer();
		}

		// ===============================================
		// NON-PUBLIC METHODS
		private void StartTimer() {
			if( sysTimer.Enabled ) return;

			sysTimer.Enabled = true;
			sysTimer.AutoReset = true;
			sysTimer.Elapsed += OnTimeout;
			sysTimer.Start();
		}

		private void StopTimer() {
			if( !sysTimer.Enabled ) return;

			sysTimer.Enabled = false;
			sysTimer.AutoReset = false;
			sysTimer.Elapsed -= OnTimeout;
			sysTimer.Stop();
		}
		
		// ===============================================
		// EVENTS
		public delegate void IOWebEvent();
		public event IOWebEvent EventSuccess;
		public event IOWebEvent EventFailure;

		// ===============================================
		// CALLBACKS
		private void OnTimeout(object sender, ElapsedEventArgs e) {
			sysContext.Send(state => OnError(), null);
		}
		
		private void OnError() {
			sAccessToken = null;
			sRefreshToken = null;
			Stop();
			EventFailure?.Invoke();
		}
		
		private void OnAuthorization(object sender, AuthSuccessEventArgs e) {
			if( string.IsNullOrEmpty(e.AccessToken) || string.IsNullOrEmpty(e.RefreshToken) ) {
				OnError();
			}
			else {
				sAccessToken = e.AccessToken;
				sRefreshToken = e.RefreshToken;
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