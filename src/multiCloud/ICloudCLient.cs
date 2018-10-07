using System;
using System.Collections.Generic;

namespace multiCloud {
	/// <summary>
	/// Represents an access to files and folders in the cloud
	/// </summary>
	public interface ICloudClient : IDisposable {
		/// <summary>
		/// Gets the list of files and folders under a given parent folder.
		/// If no folder Id is given, root level will be considered.
		/// </summary>
		/// <param name="folderId"></param>
		/// <returns></returns>
		IList<File> GetFiles(string folderId = null);

		/// <summary>
		/// Downloads a file to the given path.
		/// If no path is given, file will be downloaded in a temp folder.
		/// </summary>
		/// <param name="fileId"></param>
		/// <param name="filePath"></param>
		/// <returns>PAth to the downloaded file</returns>
		string DownloadFile(string fileId, string filePath = null);
	}
}
