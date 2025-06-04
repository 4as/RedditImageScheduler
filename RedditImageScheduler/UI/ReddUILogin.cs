using System;
using Eto.Drawing;
using Eto.Forms;
using RedditImageScheduler.UI.Login;

namespace RedditImageScheduler.UI {
	public class ReddUILogin : Form {
		private readonly ReddUILoginForm uiLoginForm = new ReddUILoginForm();
		private readonly ReddUILoginStatus uiLoginStatus = new ReddUILoginStatus();
		public ReddUILogin() {
			Title = ReddLanguage.NAME_LOGIN;
			Resizable = false;
			Size = new Size(400, 146);
			Content = uiLoginForm;
			Padding = new Padding(2);
		}
		
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			uiLoginForm.EventLogin += OnLogin;
			uiLoginForm.EventRegister += OnRegister;
			uiLoginStatus.EventCancel += OnCancel;
		}

		protected override void OnUnLoad(EventArgs e) {
			uiLoginForm.EventLogin -= OnLogin;
			uiLoginForm.EventRegister -= OnRegister;
			uiLoginStatus.EventCancel -= OnCancel;
			base.OnUnLoad(e);
		}

		public bool StatusMode {
			get => Content != uiLoginForm;
			set {
				if( value ) Content = uiLoginStatus;
				else Content = uiLoginForm;
			}
		}
		
		public string AppId => uiLoginForm.AppId;
		public string AppSecret => uiLoginForm.AppSecret;
		
		public string RedirectUrl {
			get => uiLoginForm.RedirectUrl;
			set => uiLoginForm.RedirectUrl = value;
		}

		public delegate void UILoginEvent();
		public event UILoginEvent EventLogin;
		public event UILoginEvent EventRegister;
		public event UILoginEvent EventCancel;
		
		private void OnLogin() {
			EventLogin?.Invoke();
		}
		
		private void OnCancel() {
			EventCancel?.Invoke();
		}
		
		private void OnRegister() {
			EventRegister?.Invoke();
		}
	}
}