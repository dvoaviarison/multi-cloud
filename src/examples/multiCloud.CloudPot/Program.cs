using System;
using System.Windows;

namespace multiCloud.CloudPot {
	static class Program {
		[STAThread]
		static void Main() {

			try {
				var app = new Application();
				app.Startup += (s, e) => { Bootstrap(); };
				app.Run();
			} finally {
			}
		}

		private static void Bootstrap() {
			var view = new MainWindow.View {
				ViewModel = new MainWindow.ViewModel()
			};
			view.ShowDialog();
		}
	}
}
