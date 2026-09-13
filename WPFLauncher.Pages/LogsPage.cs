using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Microsoft.Win32;
using iNKORE.UI.WPF.Modern.Controls;

namespace WPFLauncher.Pages;

public class LogsPage : iNKORE.UI.WPF.Modern.Controls.Page, IComponentConnector
{
	internal TextBox LogTextBox;

	internal Button ExportLogButton;

	internal Button ClearLogButton;

	private bool _contentLoaded;

	public static LogsPageViewModel ViewModel { get; private set; }

	public LogsPage()
	{
		InitializeComponent();
		if (ViewModel == null)
		{
			ViewModel = new LogsPageViewModel();
		}
		base.DataContext = ViewModel;
		ViewModel.IsInfoBarOpen = false;
	}

	public static void InitializeViewModel()
	{
		if (ViewModel == null)
		{
			ViewModel = new LogsPageViewModel();
		}
	}

	private void ExportLogButton_Click(object sender, RoutedEventArgs e)
	{
		try
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				Filter = "文本文件 (*.txt)|*.txt|所有文件 (*.*)|*.*",
				FileName = $"日志_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
			};
			if (saveFileDialog.ShowDialog() == true)
			{
				File.WriteAllText(saveFileDialog.FileName, ViewModel.Logs);
				iNKORE.UI.WPF.Modern.Controls.MessageBox.Show("日志已成功导出！", "导出成功", MessageBoxButton.OK, MessageBoxImage.Asterisk);
			}
		}
		catch (Exception ex)
		{
			iNKORE.UI.WPF.Modern.Controls.MessageBox.Show("导出日志时发生错误：" + ex.Message, "导出失败", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	private void ClearLogButton_Click(object sender, RoutedEventArgs e)
	{
		if (iNKORE.UI.WPF.Modern.Controls.MessageBox.Show("确定要清空所有日志吗？此操作不可撤销。", "确认清空日志", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
		{
			ViewModel.Logs = "";
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "9.0.12.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/WPFLauncher;component/pages/logspage.xaml", UriKind.Relative);
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
			LogTextBox = (TextBox)target;
			break;
		case 2:
			ExportLogButton = (Button)target;
			ExportLogButton.Click += ExportLogButton_Click;
			break;
		case 3:
			ClearLogButton = (Button)target;
			ClearLogButton.Click += ClearLogButton_Click;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
