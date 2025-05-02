using System;
using Eto.Forms;
using RedditImageScheduler.UI;

namespace RedditImageScheduler {
	public static class ReddDebug {
		public static ReddUIConsole CONSOLE;

		public static void Trace<T>(T o) => Trace(o.ToString());
		public static void Trace(string text) {
			if( CONSOLE == null ) {
				CONSOLE = new ReddUIConsole();
#if DEBUG
				CONSOLE.Closed += OnClose;
				Application.Instance.Terminating += OnClose;
				CONSOLE.Show();
#endif
			}
			CONSOLE.Write(text);
		}

		

		private static void OnClose(object sender, EventArgs e) {
			Application.Instance.Terminating -= OnClose;
			CONSOLE.Closed -= OnClose;
			CONSOLE = null;
		}
	}
}