using System;
using Eto.Drawing;
using Eto.Forms;

namespace RedditImageScheduler.UI.Login {
	public class ReddUILoginForm : DynamicLayout {
		private readonly Label etoIdLabel = new Label();
		private readonly TextBox etoId = new TextBox();
		private readonly Label etoSecretLabel = new Label();
		private readonly TextBox etoSecret = new TextBox();
		private readonly Label etoRedirectLabel = new Label();
		private readonly TextBox etoRedirect = new TextBox();
		private readonly Button etoLogin = new Button();
		private readonly Button etoRegister = new Button();
		
		public ReddUILoginForm() {
			Spacing = new Size(2, 2);
			BeginVertical();
			
			DynamicLayout login = new DynamicLayout();
			login.BeginVertical();
			
			login.BeginHorizontal();
			etoIdLabel.VerticalAlignment = VerticalAlignment.Center;
			etoIdLabel.Text = ReddLanguage.LABEL_APP_ID;
			login.Add(etoIdLabel);
			login.Add(etoId);
			login.EndHorizontal();
			
			login.BeginHorizontal();
			etoSecretLabel.VerticalAlignment = VerticalAlignment.Center;
			etoSecretLabel.Text = ReddLanguage.LABEL_APP_SECRET;
			login.Add(etoSecretLabel);
			login.Add(etoSecret);
			login.EndHorizontal();
			
			login.BeginHorizontal();
			etoRedirectLabel.VerticalAlignment = VerticalAlignment.Center;
			etoRedirectLabel.Text = ReddLanguage.LABEL_APP_REDIRECT;
			login.Add(etoRedirectLabel);
			etoRedirect.ReadOnly = true;
			login.Add(etoRedirect);
			login.EndHorizontal();
			
			login.EndVertical();
			Add(login);

			AddSpace();
			
			DynamicLayout buttons = new DynamicLayout();
			buttons.BeginHorizontal();
			etoLogin.Text = ReddLanguage.BUTTON_LOGIN;
			buttons.Add(etoLogin);
			buttons.AddSpace();
			etoRegister.Text = ReddLanguage.BUTTON_REGISTER;
			buttons.Add(etoRegister);
			buttons.EndHorizontal();
			
			Add(buttons);
		}
		
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			etoLogin.Click += OnLogin;
			etoRegister.Click += OnRegister;
		}

		protected override void OnUnLoad(EventArgs e) {
			etoLogin.Click -= OnLogin;
			etoRegister.Click -= OnRegister;
			base.OnUnLoad(e);
		}
		
		public string AppId => etoId.Text;
		public string AppSecret => etoSecret.Text;

		public string RedirectUrl {
			get => etoRedirect.Text;
			set => etoRedirect.Text = value;
		}

		public event Action EventLogin;
		public event Action EventRegister;
		
		private void OnLogin(object sender, EventArgs e) {
			EventLogin?.Invoke();
		}

		private void OnRegister(object sender, EventArgs e) {
			EventRegister?.Invoke();
		}
	}
}