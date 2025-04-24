using System;
using System.ComponentModel;
using Eto.Forms;
using RedditImageScheduler.UI;

namespace RedditImageScheduler.WinForms {
	class Program {
		[STAThread]
		public static void Main(string[] args) {
			new ReddApplication(Eto.Platforms.WinForms).Run();
		}
	}
}