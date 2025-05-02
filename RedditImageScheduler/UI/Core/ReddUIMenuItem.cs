using System;
using Eto.Forms;

namespace RedditImageScheduler.UI.Core {
	public class ReddUIMenuItem : ButtonMenuItem {
		protected override void OnValidate(EventArgs e) {
			base.OnValidate(e);
			
			int idx = Text.IndexOf('&');
			if( idx != -1 && Keys.TryParse<Keys>(Text[idx + 1].ToString(), out var key) ) {
				Shortcut = Application.Instance.CommonModifier | key;
			}
		}
	}
}