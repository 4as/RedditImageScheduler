using Reddit.AuthTokenRetriever.EventArgs;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Eto;
using Eto.Forms;
using Reddit.AuthTokenRetriever;
using uhttpsharp;
using uhttpsharp.Listeners;
using uhttpsharp.RequestProviders;
using Timer = System.Timers.Timer;

namespace RedditImageScheduler.IO.Web {
	/// <summary>
	/// Re-implementation of AuthTokenRetrieverLib to handle proper re-use.
	/// </summary>
	public class ReddIOAuth {
		//public static readonly string SCOPE = "creddits%20modcontributors%20modmail%20modconfig%20subscribe%20structuredstyles%20vote%20wikiedit%20mysubreddits%20submit%20modlog%20modposts%20modflair%20save%20modothers%20read%20privatemessages%20report%20identity%20livemanage%20account%20modtraffic%20wikiread%20edit%20modwiki%20modself%20history%20flair";
		public static readonly string SCOPE = "vote%20mysubreddits%20submit%20read%20identity%20account";
		private static readonly NamespaceInfo INFO = new NamespaceInfo("RedditImageScheduler.Resources", Assembly.GetExecutingAssembly());
		
		private readonly Timer sysTimer = new Timer(30000);
		private readonly HttpRequestProvider httpRequestProvider;
		
		private readonly string sRedirectUrl;
		private readonly string sHost;
		private readonly int nPort;
		
		private SynchronizationContext sysContext = SynchronizationContext.Current;
		private HttpServer httpServer;
		private TcpListener httpListener;
		private TcpListenerAdapter httpAdapter;
		private Process sysProcess;

		private string sScope;
		private string sAccessToken;
		private string sRefreshToken;

		public ReddIOAuth(string host = null, int port = 8080) {
			nPort = port;
			sHost = host ?? IPAddress.Loopback.ToString();
			sRedirectUrl = "http://" + sHost + ":" + nPort + "/Reddit.NET/oauthRedirect";
			sScope = SCOPE;

			httpRequestProvider = new HttpRequestProvider();
		}

		// ===============================================
		// GETTERS / SETTERS
		public bool IsListening => httpServer != null;
		public string RedirectUrl => sRedirectUrl;
		public string AccessToken => sAccessToken;
		public string RefreshToken => sRefreshToken;

		public string Scope {
			get => sScope;
			set => sScope = value;
		}

		// ===============================================
		// PUBLIC METHODS
		public void Request(string app_id, string app_secret) {
			Close();
			
			if( httpServer == null ) {
				httpServer = new HttpServer(httpRequestProvider);
				httpListener = new TcpListener(IPAddress.Parse(sHost), nPort);
				httpAdapter = new TcpListenerAdapter(httpListener);

				httpServer.Use(httpAdapter);
				httpServer.Use(Process);
				httpServer.Start();
			}

			if( sysContext == null ) sysContext = SynchronizationContext.Current;

			StartTimer();
			
			sysProcess = ReddIOWeb.OpenBrowser(GetAuthUrl(app_id, app_secret));
			sysProcess.Exited += OnClose;
		}

		public void Close() {
			if( sysProcess != null ) {
				sysProcess.Exited -= OnClose;
				sysProcess.Close();
				sysProcess = null;
			}

			if( httpServer != null ) {
				httpListener.Stop();
				httpAdapter.Dispose();
				httpServer.Dispose();

				httpListener = null;
				httpAdapter = null;
				httpServer = null;
			}

			StopTimer();
		}

		// ===============================================
		// NON-PUBLIC METHODS
		protected Task Process(IHttpContext context, Func<Task> next) {
			string code = null;
			string state = null;
			try {
				code = context.Request.QueryString.GetByName("code");
				state = context.Request.QueryString.GetByName("state");
			}
			catch( KeyNotFoundException ) {
				context.Response = new uhttpsharp.HttpResponse(HttpResponseCode.Ok, Encoding.UTF8.GetBytes("<b>ERROR:  No code and/or state received!</b>"), false);
				OnError("Invalid response received.");
			}

			if( !string.IsNullOrWhiteSpace(code)
				&& !string.IsNullOrWhiteSpace(state) ) {
				RestRequest restRequest = new RestRequest("/api/v1/access_token", Method.POST);

				restRequest.AddHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(state)));
				restRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");

				restRequest.AddParameter("grant_type", "authorization_code");
				restRequest.AddParameter("code", code);
				restRequest.AddParameter("redirect_uri", sRedirectUrl);

				OAuthToken oAuthToken = JsonConvert.DeserializeObject<OAuthToken>(ExecuteRequest(restRequest));
				
				sAccessToken = oAuthToken.AccessToken;
				sRefreshToken = oAuthToken.RefreshToken;

				OnResponse(sAccessToken, sRefreshToken);
				
				string html;
				using( Stream stream = INFO.FindResource("success.html") ) {
					using( StreamReader streamReader = new StreamReader(stream) ) {
						html = streamReader.ReadToEnd();
					}
				}

				html = html.Replace("REDDIT_OAUTH_ACCESS_TOKEN", oAuthToken.AccessToken);
				html = html.Replace("REDDIT_OAUTH_REFRESH_TOKEN", oAuthToken.RefreshToken);
				html = html.Replace("TOKEN_SAVED", "");

				context.Response = new uhttpsharp.HttpResponse(HttpResponseCode.Ok, Encoding.UTF8.GetBytes(html), false);
			}

			return Task.Factory.GetCompleted();
		}
		
		protected string ExecuteRequest(RestRequest restRequest) {
			IRestResponse res = new RestClient("https://www.reddit.com").Execute(restRequest);
			if( res != null && res.IsSuccessful ) {
				return res.Content;
			}
			else {
				Exception ex = new Exception("API returned non-success response.");

				ex.Data.Add("res", res);

				throw ex;
			}
		}

		protected string GetAuthUrl(string app_id, string app_secret) {
			return "https://www.reddit.com/api/v1/authorize?client_id=" + app_id + "&response_type=code"
				   + "&state=" + app_id + ":" + app_secret
				   + "&redirect_uri=" + sRedirectUrl + "&duration=permanent"
				   + "&scope=" + sScope;
		}
		
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
		public delegate void AuthEvent(string access_token, string refresh_token);
		public event AuthEvent EventSuccess;
		public event Action EventFailure;

		// ===============================================
		// CALLBACKS
		private void OnResponse(string access_token, string refresh_token) {
			sysContext.Send(state => EventSuccess?.Invoke(access_token, refresh_token), null);
		}
		
		private void OnClose(object sender, EventArgs e) {
			Close();
			EventFailure?.Invoke();
		}
		
		private void OnTimeout(object sender, ElapsedEventArgs e) {
			sysContext.Send(state => OnError("Request has timed out."), null);
		}

		private void OnError(string msg) {
			MessageBox.Show(msg, MessageBoxType.Error);
			Close();
			EventFailure?.Invoke();
		}
	}
}