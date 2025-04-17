using System;
using Eto.Forms;

namespace RedditImageScheduler.Gtk {
	class Program {
		[STAThread]
		public static void Main(string[] args) {
			new RedditApplication(Eto.Platforms.Gtk).Run();
		}
	}
}