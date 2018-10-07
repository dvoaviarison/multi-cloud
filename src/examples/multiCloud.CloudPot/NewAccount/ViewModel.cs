using System.Collections.Generic;
using System.Reactive;
using multiCloud.CloudPot.MainWindow;
using ReactiveUI;

namespace multiCloud.CloudPot.NewAccount {
	public class ViewModel : ReactiveObject {
		public ViewModel(List<Account> accounts) {
			AddAccount = ReactiveCommand.Create(
				execute: () => {
					accounts.Add(new Account{Name = AccountName});
				});
		}

		public ReactiveCommand<Unit, Unit> AddAccount { get; }

		private string _accountName;
		public string AccountName {
			get => _accountName;
			set => this.RaiseAndSetIfChanged(ref _accountName, value);
		}
	}
}
