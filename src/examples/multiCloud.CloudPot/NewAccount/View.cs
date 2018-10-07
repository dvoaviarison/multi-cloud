using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ReactiveUI;

namespace multiCloud.CloudPot.NewAccount {
	public class View : Window, IViewFor<ViewModel> {
		public static readonly DependencyProperty ViewModelProperty;
		private Button _okButton;
		private TextBox _accountName;

		static View() {
			ViewModelProperty = DependencyProperty.Register(
				"ViewModel",
				typeof(ViewModel),
				typeof(View));
		}

		public View() {
			InitializeResources();
			InitializeLayout();

			this.WhenActivated(d => {
				d(this.Bind(ViewModel, vm => vm.AccountName, v => v._accountName.Text));
				d(this.BindCommand(ViewModel, vm => vm.AddAccount, v => v._okButton));
				d(ViewModel.AddAccount.Subscribe(x => Close()));
			});
		}

		private void InitializeResources() {
			var vmFactory = new FrameworkElementFactory(typeof(ViewModelViewHost));
			vmFactory.SetBinding(ViewModelViewHost.ViewModelProperty, new Binding("."));
			var vmTemplate = new DataTemplate(typeof(ReactiveObject)) { VisualTree = vmFactory };
			vmTemplate.Seal();
			Resources.Add(new DataTemplateKey(typeof(ReactiveObject)), vmTemplate);
		}

		private void InitializeLayout() {
			Title = "Add Account";
			WindowStartupLocation = WindowStartupLocation.CenterScreen;
			ResizeMode = ResizeMode.NoResize;
			Width = 300;
			Height = 150;

			// Content
			_accountName = new TextBox {
				Height = 23,
				Margin = new Thickness(2)
			};

			var accountGroupBox = new GroupBox {
				Header = "Account Name",
				Margin = new Thickness(4),
				Content = _accountName
			};

			// Botton
			_okButton = new Button {
				Margin = new Thickness(2),
				Height = 23,
				Width = 50,
				Content = "Add"
			};
			var buttonStackPanel = new StackPanel {
				Orientation = Orientation.Horizontal,
				HorizontalAlignment = HorizontalAlignment.Right
			};
			buttonStackPanel.Children.Add(_okButton);

			// Main grid
			var mainGrid = new Grid();
			mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
			mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });

			mainGrid.Children.Add(accountGroupBox);
			mainGrid.Children.Add(buttonStackPanel);
			accountGroupBox.SetValue(Grid.RowProperty, 0);
			buttonStackPanel.SetValue(Grid.RowProperty, 1);

			Content = mainGrid;
		}

		public ViewModel ViewModel {
			get => (ViewModel)GetValue(ViewModelProperty);
			set => SetValue(ViewModelProperty, value);
		}

		object IViewFor.ViewModel {
			get => ViewModel;
			set => ViewModel = (ViewModel)value;
		}
	}
}
