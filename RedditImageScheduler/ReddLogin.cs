using System.Diagnostics;
using RedditImageScheduler.Data;
using RedditImageScheduler.IO;
using RedditImageScheduler.UI;

namespace RedditImageScheduler {
	public class ReddLogin {
		private readonly ReddIOWeb ioWeb;
		private ReddUILogin uiLogin;
		private ReddDataOptions dataOptions;

		public ReddLogin(ReddIOWeb web) {
			ioWeb = web;
		}
		
		public void Initialize(ReddDataOptions options) {
			Deinitialize();
			
			dataOptions = options;
			
			Show(true);

			ioWeb.EventSuccess += OnSuccess;
			ioWeb.EventFailure += OnFailure;
			ioWeb.Start(dataOptions.AppId, dataOptions.AppSecret);
		}

		public void Deinitialize() {
			ioWeb.EventSuccess -= OnSuccess;
			ioWeb.EventFailure -= OnFailure;
			ioWeb.Stop();

			dataOptions = null;

			Hide();
		}

		// ===============================================
		// GETTERS / SETTERS
		public bool IsInitialized => dataOptions != null;

		// ===============================================
		// NON-PUBLIC METHODS
		protected void Show(bool status) {
			if( uiLogin == null ) {
				uiLogin = new ReddUILogin();
				uiLogin.EventLogin += OnLogin;
				uiLogin.EventRegister += OnRegister;
				uiLogin.EventCancel += OnCancel;
				uiLogin.Show();
			}

			uiLogin.StatusMode = status;
		}

		protected void Hide() {
			if( uiLogin != null ) {
				uiLogin.EventLogin -= OnLogin;
				uiLogin.EventRegister -= OnRegister;
				uiLogin.EventCancel -= OnCancel;
			}
		}

		// ===============================================
		// EVENTS
		public delegate void UILoginEvent();
		public event UILoginEvent EventLogin;

		// ===============================================
		// CALLBACKS
		private void OnFailure() {
			ioWeb.Stop();
			Show(false);
		}

		private void OnSuccess() {
			dataOptions.SetApp(uiLogin.AppId, uiLogin.AppSecret);
			ioWeb.Stop();
			EventLogin?.Invoke();
		}
		
		private void OnLogin() {
			Show(true);
			ioWeb.Start(uiLogin.AppId, uiLogin.AppSecret);
		}

		private void OnRegister() {
			ReddIOWeb.OpenBrowser(ReddConfig.URL_REGISTER);
		}
		
		private void OnCancel() {
			ioWeb.Stop();
			Show(false);
		}
	}
}