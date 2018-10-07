![HeadBanner](docs/imgs/headbanner.png)
[![Build status](https://ci.appveyor.com/api/projects/status/gam48s84441rlcso/branch/master?svg=true)](https://ci.appveyor.com/project/dvoaviarison/multi-cloud/branch/master)
# Multi Cloud
Multi Cloud is an open source .Net library that aims to provide with extremely simplified access to multiple cloud drives through the same interface.
That includes:
- Encapsulation of authentification. All you need is to provice you application [API credential file](https://cloud.google.com/genomics/docs/how-tos/getting-started)
- Getting list of files, and very easily navigate through the folder trees.
- Downloading/uploading files etc...


## Get started
Very simple, the multi cloud client interface looks like this

```csharp
	public interface ICloudClient : IDisposable {
		IList<File> GetFiles(string folderId = null);
		string DownloadFile(string fileId, string filePath = null);
		...
	}
```

Working examples are available in `src` folder.

## Contribute
This project is open source. Fork then PR!

For now we are supporting Google Drive. We will shortly introduce OneDrive as well.

## Compatibility
- .Net Core >= netCore2.0
