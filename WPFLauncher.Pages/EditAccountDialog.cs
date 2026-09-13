using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using WPFLauncher.DataTypes;
using iNKORE.UI.WPF.Modern.Controls;

namespace WPFLauncher.Pages;

public class EditAccountDialog : ContentDialog, IComponentConnector
{
	private readonly Account _originalAccount;

	internal TextBox AccountNameTextBox;

	internal TextBox PasswordTextBox;

	internal ComboBox AccountTypeComboBox;

	private bool _contentLoaded;

	public EditAccountDialog(Account account)
	{
		InitializeComponent();
		_originalAccount = account;
		LoadAccountData();
	}

	private void LoadAccountData()
	{
		AccountNameTextBox.Text = _originalAccount.UserName;
		PasswordTextBox.Text = _originalAccount.Password;
		switch (_originalAccount.AccountType)
		{
		case AccountTypes.Netease:
			AccountTypeComboBox.SelectedIndex = 0;
			break;
		case AccountTypes._4399:
			AccountTypeComboBox.SelectedIndex = 1;
			break;
		case AccountTypes.Cookie:
			AccountTypeComboBox.SelectedIndex = 2;
			break;
		}
	}

	public Account GetUpdatedAccount()
	{
		return new Account
		{
			UserName = AccountNameTextBox.Text.Trim(),
			Password = PasswordTextBox.Text.Trim(),
			AccountType = GetSelectedAccountType()
		};
	}

	private AccountTypes GetSelectedAccountType()
	{
		return AccountTypeComboBox.SelectedIndex switch
		{
			0 => AccountTypes.Netease, 
			1 => AccountTypes._4399, 
			2 => AccountTypes.Cookie, 
			_ => AccountTypes.Netease, 
		};
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "9.0.12.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/WPFLauncher;component/dialog/editaccountdialog.xaml", UriKind.Relative);
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
			AccountNameTextBox = (TextBox)target;
			break;
		case 2:
			PasswordTextBox = (TextBox)target;
			break;
		case 3:
			AccountTypeComboBox = (ComboBox)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
