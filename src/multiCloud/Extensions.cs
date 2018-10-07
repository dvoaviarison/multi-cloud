using System.Linq;

namespace multiCloud
{
	public static class Extensions {
		public static File ToMultiCloud(this Google.Apis.Drive.v3.Data.File self) {
			return new File {
				Id = self.Id,
				Name = self.Name,
				IsFolder = self.MimeType == "application/vnd.google-apps.folder",
				ParentId = self.Parents?.First()
			};
		}
	}
}
