using System;
using Eto.Drawing;
using Eto.Forms;
using RedditImageScheduler.Utils;

namespace RedditImageScheduler.UI.Entry {
	public class ReddUISource : TextBox {
		
		private readonly Color colorDefault;
		private bool isValid;

		public ReddUISource() {
			colorDefault = BackgroundColor;
			PlaceholderText = ReddLanguage.SOURCE;
		}

		public bool IsValid => isValid;

		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			TextChanged += OnModify;
			OnModify(null, null);
		}

		protected override void OnUnLoad(EventArgs e) {
			TextChanged -= OnModify;
			base.OnUnLoad(e);
		}

		// ===============================================
		// EVENTS
		public delegate void UITitleEvent();
		public event UITitleEvent OnSourceChanged;

		// ===============================================
		// CALLBACKS
		private void OnModify(object sender, EventArgs e) {
			if( string.IsNullOrEmpty(Text) || !Uri.TryCreate(Text, UriKind.Absolute, out var uriResult)
										   || (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps) ) {
				isValid = false;
				BackgroundColor = ReddConfig.UI_BG_INVALID;
			}
			else {
				isValid = true;
				BackgroundColor = colorDefault;
			}

			OnSourceChanged?.Invoke();
		}
	}
}