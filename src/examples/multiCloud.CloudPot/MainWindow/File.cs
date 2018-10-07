namespace multiCloud.CloudPot.MainWindow
{
	public class File {
		public bool IsFolder { get; set; } = false;
		public string Name { get; set; }
		public string Id { get; set; }
		public string ParentId { get; set; }

		public string AccountName { get; set; }
	}
}
