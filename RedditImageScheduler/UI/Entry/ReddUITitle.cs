using System;
using Eto.Drawing;
using Eto.Forms;

namespace RedditImageScheduler.UI.Entry {
	public class ReddUITitle : TextBox {
		private readonly Color colorDefault;
		private bool isValid;

		public ReddUITitle() {
			colorDefault = BackgroundColor;
			PlaceholderText = ReddLanguage.TITLE;
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
		public event UITitleEvent OnTitleChanged;
		
		// ===============================================
		// CALLBACKS
		private void OnModify(object sender, EventArgs e) {
			if( string.IsNullOrEmpty(Text) || Text.Length < ReddConfig.ENTRY_TITLE_LENGTH ) {
				isValid = false;
				BackgroundColor = ReddConfig.UI_BG_INVALID;
			}
			else {
				isValid = true;
				BackgroundColor = colorDefault;
			}
			OnTitleChanged?.Invoke();
		}
	}
}