namespace multiCloud {
	/// <summary>
	/// Represents a file or a folder in the cloud
	/// </summary>
	public class File {
		/// <summary>
		/// NAme of the file or folder
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Id of the file or folder
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Parent Id of the file or folder
		/// </summary>
		public string ParentId { get; set; }

		/// <summary>
		/// Gets and sets whether it is a folder
		/// </summary>
		public bool IsFolder { get; set; }
	}
}
