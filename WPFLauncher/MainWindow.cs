using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Windows;
using System.Windows.Markup;
using Mcl.Core.Utils;
using Newtonsoft.Json.Linq;
using WPFLauncher.Network.Http;
using WPFLauncher.Pages;
using iNKORE.UI.WPF.Modern.Controls;

namespace WPFLauncher;

public class MainWindow : Window, IComponentConnector
{
	public HomePage Page_Home = new HomePage();

	public AccountsManagePage Page_Accounts = new AccountsManagePage();

	public SettingsPage Page_Settings = new SettingsPage();

	public LogsPage Page_Logs = new LogsPage();

	public ItemList Page_ItemList = new ItemList();

	internal NavigationView NavigationView_Root;

	internal NavigationViewItem NavigationViewItem_Home;

	internal NavigationViewItem NavigationViewItem_Accounts;

	internal NavigationViewItem NavigationViewItem_ItemList;

	internal NavigationViewItem NavigationViewItem_Logs;

	internal Frame Frame_Main;

	private bool _contentLoaded;

	public MainWindow()
	{
		InitializeComponent();
	}

	private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
	{
		object selectedItem = sender.SelectedItem;
		Page page = null;
		if (selectedItem == NavigationViewItem_Home)
		{
			page = Page_Home;
		}
		else if (selectedItem == NavigationViewItem_Accounts)
		{
			page = Page_Accounts;
		}
		else if (selectedItem == NavigationViewItem_Logs)
		{
			page = Page_Logs;
		}
		else if (selectedItem == NavigationViewItem_ItemList)
		{
			if (!Var.CurrentAccount.HasLogin)
			{
				Function.ShowDialog("未登录,请先登录后再操作", "错误");
				NavigationView_Root.SelectedItem = NavigationViewItem_Accounts;
				return;
			}
			page = Page_ItemList;
		}
		else if (args.IsSettingsSelected)
		{
			page = Page_Settings;
		}
		if (page != null)
		{
			NavigationView_Root.Header = page.Title;
			Frame_Main.Navigate(page);
		}
	}

	private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
	{
		NavigationView_Root.SelectedItem = NavigationViewItem_Home;
		try
		{
			using HttpClient httpClient = new HttpClient();
			JObject jObject = JObject.Parse(await (await httpClient.GetAsync("https://x19.update.netease.com/serverlist/release.json")).Content.ReadAsStringAsync());
			X19HttpLogin.WebServerUrl = jObject["CoreServerUrl"]?.ToString() ?? string.Empty;
			X19Http.WebServerUrl = jObject["ApiGatewayUrl"]?.ToString() ?? string.Empty;
			Logger.Default.Info("成功获取到服务器列表, CoreServerUrl: " + X19HttpLogin.CoreServerUrl + ", ApiGatewayUrl: " + X19Http.WebServerUrl);
		}
		catch (Exception ex)
		{
			Logger.Default.Error("获取服务器列表失败: " + ex.Message);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "9.0.12.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/WPFLauncher;component/mainwindow.xaml", UriKind.Relative);
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
			((MainWindow)target).Loaded += MainWindow_OnLoaded;
			break;
		case 2:
			NavigationView_Root = (NavigationView)target;
			NavigationView_Root.SelectionChanged += NavigationView_SelectionChanged;
			break;
		case 3:
			NavigationViewItem_Home = (NavigationViewItem)target;
			break;
		case 4:
			NavigationViewItem_Accounts = (NavigationViewItem)target;
			break;
		case 5:
			NavigationViewItem_ItemList = (NavigationViewItem)target;
			break;
		case 6:
			NavigationViewItem_Logs = (NavigationViewItem)target;
			break;
		case 7:
			Frame_Main = (Frame)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
