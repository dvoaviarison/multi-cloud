using System;

namespace multiCloud.Examples {
	class Program {
		static void Main(string[] args) {

			Console.WriteLine("Multi-Cloud Example");

			while (true) {
				var client = new GoogleDriveClient(
					name: "GoogleAccount",
					applicationName: "Multi-Cloud Example",
					credentialJsonFile: "credential.json"
				);

				var files = client.GetFiles(null);
				if (files != null)
					foreach (var file in files)
						Console.WriteLine($"> {file.Name}");
				// client.Dispose();

				Console.WriteLine("Any key to restart...");
				Console.ReadKey();
			}
		}
	}
}
