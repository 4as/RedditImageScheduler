using System;
using Eto.Forms;

namespace RedditImageScheduler.Gtk {
	class Program {
		[STAThread]
		public static void Main(string[] args) {
			new ReddApplication(Eto.Platforms.Gtk).Run();
		}
	}
}