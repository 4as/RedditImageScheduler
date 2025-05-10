using System;
using Eto.Drawing;
using Eto.Forms;
using RedditImageScheduler.Utils;

namespace RedditImageScheduler.UI.Editor {
	public class ReddUISource : TextBox {
		
		private readonly Color colorDefault;
		private bool isValid;

		public ReddUISource() {
			colorDefault = BackgroundColor;
			PlaceholderText = ReddLanguage.TEXT_PLACEHOLDER_SOURCE;
		}

		public bool IsValid => isValid;
		
		// ===============================================
		// PUBLIC METHODS
		public void Refresh() => OnValidate();

		// ===============================================
		// NON-PUBLIC METHODS
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
		private void OnValidate() {
			if( ReddValidator.IsValidSource(Text) ) {
				isValid = true;
				BackgroundColor = colorDefault;
			}
			else {
				isValid = false;
				BackgroundColor = ReddConfig.UI_BG_INVALID;
			}
		}
		
		private void OnModify(object sender, EventArgs e) {
			OnValidate();
			OnSourceChanged?.Invoke();
		}
	}
}