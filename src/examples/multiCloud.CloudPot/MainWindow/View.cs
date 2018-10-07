using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using ReactiveUI;

namespace multiCloud.CloudPot.MainWindow {
	public class View : Window, IViewFor<ViewModel> {
		public static readonly DependencyProperty ViewModelProperty;
		private DataGrid _accountDataGrid;
		private DataGrid _contentDataGrid;
		private Button _addAccountButton;
		private Button _removeAccountButton;
		private Button _goUpButton;

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
				d(this.OneWayBind(ViewModel, vm => vm.Accounts, v => v._accountDataGrid.ItemsSource));
				d(this.OneWayBind(ViewModel, vm => vm.Files, v => v._contentDataGrid.ItemsSource));
				d(this.Bind(ViewModel, vm => vm.SelectedAccount, v => v._accountDataGrid.SelectedItem));
				d(this.Bind(ViewModel, vm => vm.SelectedFile, v => v._contentDataGrid.SelectedItem));
				d(this.BindCommand(ViewModel, vm => vm.RemoveAccount, v => v._removeAccountButton));
				d(this.BindCommand(ViewModel, vm => vm.AddAccount, v => v._addAccountButton));
				d(this.BindCommand(ViewModel, vm => vm.GoUp, v => v._goUpButton));
				//d(ViewModel.RemoveAccount.Subscribe(x => Close()));
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
			Title = "Cloud Pot";
			WindowStartupLocation = WindowStartupLocation.CenterScreen;
			Width = 1000;
			Height = 600;

			// Content
			_goUpButton = new Button {
				Margin = new Thickness(2),
				Height = 23,
				Width = 50,
				Content = "Up"
			};
			var contentButtonStackPanel = new StackPanel {
				HorizontalAlignment = HorizontalAlignment.Right,
				Orientation = Orientation.Horizontal
			};
			contentButtonStackPanel.Children.Add(_goUpButton);

			var rowStyle = new Style(typeof(DataGridRow));
			rowStyle.Setters.Add(new EventSetter(MouseDoubleClickEvent, new MouseButtonEventHandler(RowDoubleClick)));
			_contentDataGrid = new DataGrid {
				Margin = new Thickness(2),
				IsReadOnly = true,
				RowStyle = rowStyle
			};

			var contentGrid = new Grid();
			contentGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
			contentGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
			contentGrid.Children.Add(contentButtonStackPanel);
			contentGrid.Children.Add(_contentDataGrid);
			contentButtonStackPanel.SetValue(Grid.RowProperty, 0);
			_contentDataGrid.SetValue(Grid.RowProperty, 1);

			var contentGroupBox = new GroupBox {
				Header = "All Contents",
				Margin = new Thickness(4),
				Content = contentGrid
			};

			// Account list
			_accountDataGrid = new DataGrid {
				Margin = new Thickness(2),
				IsReadOnly = true
			};
			_removeAccountButton = new Button {
				Margin = new Thickness(2),
				Height = 23,
				Width = 50,
				Content = "Remove"
			};
			_addAccountButton = new Button {
				Margin = new Thickness(2),
				Height = 23,
				Width = 50,
				Content = "Add"
			};
			var accoutButtonStackPanel = new StackPanel {
				HorizontalAlignment = HorizontalAlignment.Right,
				Orientation = Orientation.Horizontal
			};
			accoutButtonStackPanel.Children.Add(_removeAccountButton);
			accoutButtonStackPanel.Children.Add(_addAccountButton);

			var accountGrid = new Grid();
			accountGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
			accountGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
			accountGrid.Children.Add(_accountDataGrid);
			accountGrid.Children.Add(accoutButtonStackPanel);
			_accountDataGrid.SetValue(Grid.RowProperty, 0);
			accoutButtonStackPanel.SetValue(Grid.RowProperty, 1);

			var accountGroupBox = new GroupBox {
				Header = "Accounts",
				Margin = new Thickness(4),
				Content = accountGrid
			};

			// Main grid
			var mainGrid = new Grid();
			mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
			mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) });

			mainGrid.Children.Add(accountGroupBox);
			mainGrid.Children.Add(contentGroupBox);
			accountGroupBox.SetValue(Grid.ColumnProperty, 0);
			contentGroupBox.SetValue(Grid.ColumnProperty, 1);

			Content = mainGrid;
		}

		private void RowDoubleClick(object sender, MouseButtonEventArgs e) {
			ViewModel.EnterFolder();
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
