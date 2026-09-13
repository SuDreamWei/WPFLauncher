using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Newtonsoft.Json;
using WPFLauncher.DataTypes;
using WPFLauncher.Modules.Config;
using WPFLauncher.Modules.Utils;
using WPFLauncher.ViewModels;
using iNKORE.UI.WPF.Modern.Controls;

namespace WPFLauncher.Pages;

public class AccountsManagePage : iNKORE.UI.WPF.Modern.Controls.Page, INotifyPropertyChanged, IComponentConnector, IStyleConnector
{
	private Account _selectedAccount;

	internal iNKORE.UI.WPF.Modern.Controls.ListView AccountsList;

	private bool _contentLoaded;

	public ObservableCollection<Account> Accounts { get; set; }

	public Account SelectedAccount
	{
		get
		{
			return _selectedAccount;
		}
		set
		{
			_selectedAccount = value;
			OnPropertyChanged("SelectedAccount");
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	public AccountsManagePage()
	{
		InitializeComponent();
		Accounts = new ObservableCollection<Account>();
		base.DataContext = this;
		LoadAccounts();
	}

	private void LoadAccounts()
	{
		string text = Config.ReadConfig(Config.ConfigName.Account);
		if (string.IsNullOrWhiteSpace(text) || !Tools.IsVaildJson(text))
		{
			return;
		}
		try
		{
			ObservableCollection<Account> observableCollection = JsonConvert.DeserializeObject<ObservableCollection<Account>>(text);
			if (observableCollection == null)
			{
				return;
			}
			Accounts.Clear();
			foreach (Account item in observableCollection)
			{
				Accounts.Add(item);
			}
		}
		catch
		{
		}
	}

	private void SaveAccounts()
	{
		try
		{
			string contents = JsonConvert.SerializeObject(Accounts, Formatting.Indented);
			File.WriteAllText(Path.Combine(Config.ConfigDirectory, Config.ConfigName.Account), contents);
		}
		catch (Exception ex)
		{
			Function.ShowDialog("保存账号信息失败: " + ex.Message, "错误");
		}
	}

	private async void EditButton_Click(object sender, RoutedEventArgs e)
	{
		if (!(((sender is Button button) ? button.Tag : null) is Account account))
		{
			return;
		}
		EditAccountDialog dialog = new EditAccountDialog(account);
		if (await dialog.ShowAsync() == ContentDialogResult.Primary)
		{
			Account updatedAccount = dialog.GetUpdatedAccount();
			account.UserName = updatedAccount.UserName;
			account.Password = updatedAccount.Password;
			account.AccountType = updatedAccount.AccountType;
			LoginProgressDialog progressDialog = new LoginProgressDialog(account);
			await progressDialog.ShowAsync();
			bool num = account.HasLogin && !string.IsNullOrEmpty(account.NickName);
			string text = ((progressDialog.DataContext is LoginProgressViewModel loginProgressViewModel) ? loginProgressViewModel.ErrorMessage : "");
			if (num)
			{
				SaveAccounts();
				return;
			}
			string text2 = (string.IsNullOrEmpty(text) ? "重新登录失败，请检查账号信息" : text);
			Function.AddLog("账号编辑失败: " + text2);
		}
	}

	private async void SelectButton_Click(object sender, RoutedEventArgs e)
	{
		if (!(((sender is Button button) ? button.Tag : null) is Account account))
		{
			return;
		}
		if (!account.HasLogin)
		{
			if (iNKORE.UI.WPF.Modern.Controls.MessageBox.Show("账号 \"" + account.NickName + "\" 尚未登录，是否现在登录？", "账号未登录", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
			{
				return;
			}
			LoginProgressDialog progressDialog = new LoginProgressDialog(account);
			await progressDialog.ShowAsync();
			bool hasLogin = account.HasLogin;
			string text = ((progressDialog.DataContext is LoginProgressViewModel loginProgressViewModel) ? loginProgressViewModel.ErrorMessage : "");
			if (!hasLogin)
			{
				string text2 = (string.IsNullOrEmpty(text) ? "登录失败，请检查账号信息" : text);
				Function.AddLog("账号登录失败: " + text2);
				return;
			}
		}
		Var.CurrentAccount = account;
		Function.ShowDialog("已选择账号 \"" + account.NickName + "\"", "选择成功");
	}

	private async void loginButton_Click(object sender, RoutedEventArgs e)
	{
		Button button = sender as Button;
		Account account = button?.Tag as Account;
		if (account != null && account.HasLogin)
		{
			if (iNKORE.UI.WPF.Modern.Controls.MessageBox.Show("确定要重新登录账号 \"" + account.NickName + "\" 吗？", "重新登录确认", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				account.HasLogin = false;
				LoginProgressDialog progressDialog = new LoginProgressDialog(account);
				await progressDialog.ShowAsync();
				bool hasLogin = account.HasLogin;
				string text = ((progressDialog.DataContext is LoginProgressViewModel loginProgressViewModel) ? loginProgressViewModel.ErrorMessage : "");
				if (hasLogin)
				{
					SaveAccounts();
					Function.ShowDialog("账号 \"" + account.NickName + "\" 重新登录成功", "Success");
				}
				else
				{
					string text2 = (string.IsNullOrEmpty(text) ? "重新登录失败，请检查账号信息" : text);
					Function.AddLog("账号重新登录失败: " + text2);
					Function.ShowDialog("账号 \"" + account.NickName + "\" 重新登录失败: " + text2, "Failed");
				}
			}
		}
		else
		{
			LoginProgressDialog progressDialog = new LoginProgressDialog(account);
			await progressDialog.ShowAsync();
			bool hasLogin2 = account.HasLogin;
			string text3 = ((progressDialog.DataContext is LoginProgressViewModel loginProgressViewModel2) ? loginProgressViewModel2.ErrorMessage : "");
			if (hasLogin2)
			{
				SaveAccounts();
				Function.ShowDialog("账号 \"" + account.NickName + "\" 登录成功", "Success");
				button.Content = "重新登录";
			}
			else
			{
				string text4 = (string.IsNullOrEmpty(text3) ? "登录失败，请检查账号信息" : text3);
				Function.AddLog("账号登录失败: " + text4);
				Function.ShowDialog("账号 \"" + account.NickName + "\" 登录失败: " + text4, "Failed");
			}
		}
	}

	private void DeleteButton_Click(object sender, RoutedEventArgs e)
	{
		if ((sender as Button)?.Tag is Account account && iNKORE.UI.WPF.Modern.Controls.MessageBox.Show("确定要删除账号 \"" + account.NickName + "\" 吗？此操作不可撤销。", "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
		{
			Accounts.Remove(account);
			SaveAccounts();
			Function.ShowDialog("账号 \"" + account.NickName + "\" 已删除", "删除成功");
		}
	}

	private async void AddAccountButton_Click(object sender, RoutedEventArgs e)
	{
		_ = 1;
		try
		{
			AddAccountDialog dialog = new AddAccountDialog();
			if (await dialog.ShowAsync() == ContentDialogResult.Primary)
			{
				Account newAccount = dialog.GetNewAccount();
				LoginProgressDialog progressDialog = new LoginProgressDialog(newAccount);
				await progressDialog.ShowAsync();
				bool num = newAccount.HasLogin && !string.IsNullOrEmpty(newAccount.NickName);
				string text = ((progressDialog.DataContext is LoginProgressViewModel loginProgressViewModel) ? loginProgressViewModel.ErrorMessage : "");
				if (num)
				{
					Accounts.Add(newAccount);
					SaveAccounts();
				}
				else
				{
					string text2 = (string.IsNullOrEmpty(text) ? "登录失败，请检查账号信息" : text);
					Function.AddLog("账号添加失败: " + text2);
				}
			}
		}
		catch (Exception ex)
		{
			Function.ShowDialog("添加账号失败: " + ex.Message, "错误");
			Function.AddLog($"添加账号失败: {ex} \n stacktrace: {ex.StackTrace}");
		}
	}

	private void UpdateCurrentSelectedAccount(Account selectedAccount)
	{
		foreach (Account account in Accounts)
		{
			if (account != selectedAccount)
			{
				account.IsCurrentSelected = false;
			}
		}
		selectedAccount.IsCurrentSelected = true;
		OnPropertyChanged("Accounts");
	}

	protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "9.0.12.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/WPFLauncher;component/pages/accountsmanagepage.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "9.0.12.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			((Button)target).Click += AddAccountButton_Click;
			break;
		case 2:
			AccountsList = (iNKORE.UI.WPF.Modern.Controls.ListView)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "9.0.12.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IStyleConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 3:
			((Button)target).Click += SelectButton_Click;
			break;
		case 4:
			((Button)target).Click += EditButton_Click;
			break;
		case 5:
			((Button)target).Click += loginButton_Click;
			break;
		case 6:
			((Button)target).Click += DeleteButton_Click;
			break;
		}
	}
}
