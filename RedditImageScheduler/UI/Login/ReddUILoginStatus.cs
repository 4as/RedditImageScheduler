using System;
using Eto.Drawing;
using Eto.Forms;

namespace RedditImageScheduler.UI.Login {
	public class ReddUILoginStatus : DynamicLayout {
		private readonly Label etoStatus = new Label();
		private readonly Button etoCancel = new Button();
		public ReddUILoginStatus() {
			Spacing = new Size(2, 2);
			BeginVertical();
			etoStatus.TextAlignment = TextAlignment.Center;
			etoStatus.VerticalAlignment = VerticalAlignment.Center;
			etoStatus.Text = ReddLanguage.TEXT_CONNECTING;
			Add(etoStatus, true, true);
			etoCancel.Height = 30;
			etoCancel.Text = ReddLanguage.BUTTON_CANCEL;
			Add(etoCancel, false, false);
			EndVertical();
		}

		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			etoCancel.Click += OnCancel;
		}

		protected override void OnUnLoad(EventArgs e) {
			etoCancel.Click -= OnCancel;
			base.OnUnLoad(e);
		}

		public event System.Action EventCancel;

		private void OnCancel(object sender, EventArgs e) {
			EventCancel?.Invoke();
		}
	}
}