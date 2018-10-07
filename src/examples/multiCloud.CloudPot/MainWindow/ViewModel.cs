using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using ReactiveUI;

namespace multiCloud.CloudPot.MainWindow {
	public class ViewModel : ReactiveObject {
		private readonly Dictionary<string, ICloudClient> _clients;
		private readonly Stack<string> _parents = new Stack<string>();
		private string _currentAccount = null;
		public ViewModel() {
			_clients = new Dictionary<string, ICloudClient>();

			var refreshRoot = new Action(() => {
				Files.Clear();
				foreach (var client in _clients) {
					var googleFiles = client.Value.GetFiles(null);
					googleFiles.Select(x => new File {
							Name = x.Name,
							Id = x.Id,
							IsFolder = x.IsFolder,
							ParentId = x.ParentId,
							AccountName = client.Key
						})
						.ToList()
						.ForEach(x => Files.Add(x));
				}
			});

			var addAccount = new Action<Account>(account => {
				if (_clients.ContainsKey(account.Name))
					return;
				Accounts.Add(account);
				var client = new GoogleDriveClient(
					name: account.Name,
					applicationName: "CloudPlot",
					credentialJsonFile: "credential.json");
				_clients[account.Name] = client;
				refreshRoot();
			});

			AddAccount = ReactiveCommand.Create(
			execute: () => {
				var newAccounts = new List<Account>();
				var view = new NewAccount.View {
					ViewModel = new NewAccount.ViewModel(newAccounts)
				};

				view.ShowDialog();
				if (!newAccounts.Any())
					return;

				newAccounts.ForEach(x => addAccount(x));
			});

			RemoveAccount = ReactiveCommand.Create(
				canExecute: this.WhenAny(x => x.SelectedAccount, selectedAccount => selectedAccount != null),
				execute: () => {
					if (SelectedAccount == null)
						return;
					_clients[SelectedAccount.Name].Dispose();
					_clients.Remove(SelectedAccount.Name);
					var toRemove = Files?.Where(x => x.AccountName == SelectedAccount.Name).ToList();
					toRemove?.ForEach(x => Files.Remove(x));
					Accounts.Remove(SelectedAccount);
				}
			);

			GoUp = ReactiveCommand.Create(
				execute: () => {
					if (!_parents.Any())
						return;
					var parent = _parents.Pop();
					if (_parents.Any()) {
						Files.Clear();
						var googleFiles = _clients[_currentAccount].GetFiles(parent);
						googleFiles.Select(x => new File {
								Name = x.Name,
								Id = x.Id,
								IsFolder = x.IsFolder,
								ParentId = x.ParentId,
								AccountName = _currentAccount
							})
							.ToList()
							.ForEach(x => Files.Add(x));
						return;
					}

					refreshRoot();
				}
			);
		}

		private Account _selectedAccount;

		public Account SelectedAccount {
			get => _selectedAccount;
			set => this.RaiseAndSetIfChanged(ref _selectedAccount, value);
		}

		private File _selectedFile;

		public File SelectedFile {
			get => _selectedFile;
			set => this.RaiseAndSetIfChanged(ref _selectedFile, value);
		}

		public ReactiveCommand<Unit, Unit> AddAccount { get; }

		public ReactiveCommand<Unit, Unit> RemoveAccount { get; }

		public ReactiveCommand<Unit, Unit> GoUp { get; }

		public ReactiveList<File> Files { get; private set; } = new ReactiveList<File>();

		public ReactiveList<Account> Accounts { get; } = new ReactiveList<Account>();

		public void EnterFolder() {
			var client = _clients[SelectedFile.AccountName];
			if (SelectedFile.IsFolder) {
				_currentAccount = SelectedFile.AccountName;
				_parents.Push(SelectedFile.ParentId);
				var googleFiles = client.GetFiles(SelectedFile.Id);
				var newFiles = googleFiles?.Select(x => new File {
					Name = x.Name,
					Id = x.Id,
					IsFolder = x.IsFolder,
					ParentId = x.ParentId,
					AccountName = SelectedFile.AccountName
				}).ToList();
				Files.Clear();
				newFiles?.ForEach(x => Files.Add(x));
				return;
			}

			var downloadedFile = client.DownloadFile(SelectedFile.Id);
			if (downloadedFile != null)
				Process.Start(downloadedFile);
		}
	}
}
