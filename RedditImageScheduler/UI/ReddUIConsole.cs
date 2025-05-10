using System.Collections.Generic;
using System.Text;
using Eto.Drawing;
using Eto.Forms;

namespace RedditImageScheduler.UI {
	public class ReddUIConsole : Form {
		private readonly List<string> listEntries = new List<string>();
		private readonly TextArea etoText = new TextArea();
		private readonly StringBuilder stringBuilder = new StringBuilder();
		public ReddUIConsole() {
			ShowInTaskbar = false;
			Size = new Size(600, 400);
			etoText.BackgroundColor = new Color(0f, 0f, 0f, 1f);
			etoText.TextColor = new Color(1f, 1f, 1f, 1f);
			etoText.ReadOnly = true;
			Content = etoText;
			Title = "Console";
		}

		public void Write(string text) {
			listEntries.Add(text);
			if( listEntries.Count > 100 ) {
				listEntries.RemoveAt(0);
			}

			stringBuilder.Clear();
			for( int i = 0, iLen = listEntries.Count; i < iLen; i++ ) {
				stringBuilder.AppendLine(listEntries[i]);
			}
			
			etoText.Text = stringBuilder.ToString();
		}
	}
}