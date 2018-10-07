using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace multiCloud {
	/// <summary>
	/// Represents an access to files and folders in google drive
	/// </summary>
	public class GoogleDriveClient : ICloudClient {
		private readonly DriveService _service;
		private readonly IDataStore _tokenStore;
		private readonly string _tokenLocationDirName;

		public GoogleDriveClient(
			string name,
			string applicationName,
			string credentialJsonFile) {
			_tokenLocationDirName = $"{applicationName}_{name}";
			_tokenStore = new FileDataStore(_tokenLocationDirName);
			_service = CreateService(
				name: name,
				applicationName: applicationName,
				credentialJsonFile:credentialJsonFile,
				tokenStore: _tokenStore);
		}

		/// <inheritdoc/>
		public IList<File> GetFiles(string folderId = null) {
			folderId = folderId ?? "root";
			var listRequest = _service.Files.List();
			listRequest.Q = $"'{folderId}' in parents";
			listRequest.PageSize = 1000;
			listRequest.Fields = "nextPageToken, files(id, name, parents, mimeType)";

			// List files.
			var result = listRequest.Execute();
			return result.Files?.Select(x => x.ToMultiCloud()).ToList();
		}

		/// <inheritdoc/>
		public string DownloadFile(string fileId, string filePath = null) {
			var request = _service.Files.Get(fileId);
			var file = request.Execute();
			if (file == null)
				return null;

			if (filePath == null)
				filePath = Path.Combine(
					Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
					_tokenLocationDirName,
					file.Name);

			using (var fileStream = System.IO.File.Create(filePath)) {
				request.Download(fileStream);
				return filePath;
			}
		}

		private DriveService CreateService(
			string name,
			string applicationName,
			string credentialJsonFile,
			IDataStore tokenStore) {

			using (var stream =
				new FileStream("credentials.json", FileMode.Open, FileAccess.Read)) {
				var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
					GoogleClientSecrets.Load(stream).Secrets,
					new[] { DriveService.Scope.DriveReadonly },
					name,
					CancellationToken.None,
					tokenStore).Result;

				return new DriveService(new BaseClientService.Initializer() {
					HttpClientInitializer = credential,
					ApplicationName = applicationName,
				});
			}
		}

		public void Dispose() {
			_service?.Dispose();
			_tokenStore?.ClearAsync();
		}
	}
}
