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
			AllowDrop = true;
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
		protected override void OnDragEnter(DragEventArgs e) {
			base.OnDragEnter(e);
			if( e.Data.ContainsUris ) {
				e.Effects = DragEffects.Copy;
			}
			else {
				e.Effects = DragEffects.None;
			}
		}

		protected override void OnDragDrop(DragEventArgs e) {
			base.OnDragDrop(e);
			if( e.Data.ContainsUris ) {
				Uri uri = e.Data.Uris[0];
				if( uri.IsFile ) {
					string url = ReddURLFileParser.GetUrl(uri);
					if( url != null ) {
						Text = url;
					}
				}
				else if( uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps ) {
					Text = uri.AbsoluteUri;
				}
			}
		}

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