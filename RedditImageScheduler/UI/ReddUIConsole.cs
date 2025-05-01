using System.Text;
using Eto.Drawing;
using Eto.Forms;

namespace RedditImageScheduler.UI {
	public class ReddUIConsole : Form {
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
			stringBuilder.AppendLine(text);
			etoText.Text = stringBuilder.ToString();
		}
	}
}