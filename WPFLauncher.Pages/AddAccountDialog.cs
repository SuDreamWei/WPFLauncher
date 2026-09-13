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

public class AddAccountDialog : ContentDialog, IComponentConnector
{
	internal TextBox AccountNameTextBox;

	internal TextBox PasswordTextBox;

	internal ComboBox AccountTypeComboBox;

	private bool _contentLoaded;

	public AddAccountDialog()
	{
		InitializeComponent();
	}

	public Account GetNewAccount()
	{
		Account account = new Account
		{
			UserName = AccountNameTextBox.Text.Trim(),
			Password = PasswordTextBox.Text.Trim()
		};
		switch (AccountTypeComboBox.SelectedIndex)
		{
		case 0:
			account.AccountType = AccountTypes.Netease;
			break;
		case 1:
			account.AccountType = AccountTypes._4399;
			break;
		case 2:
			account.AccountType = AccountTypes.Cookie;
			break;
		}
		return account;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "9.0.12.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/WPFLauncher;component/dialog/addaccountdialog.xaml", UriKind.Relative);
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
