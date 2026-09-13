using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Threading;
using Mcl.Core.Utils;
using NLog;

namespace WPFLauncher;

public class App : Application
{
	private bool _contentLoaded;

	protected override void OnStartup(StartupEventArgs e)
	{
		base.OnStartup(e);
		DispatcherHelper.Initialize();
		Dispatcher.CurrentDispatcher?.BeginInvoke(DispatcherPriority.Background, (Action)delegate
		{
			Mcl.Core.Utils.Logger.EnableUILogging(LogLevel.Info);
			Mcl.Core.Utils.Logger.Default.Info("应用程序启动");
		});
	}

	protected override void OnExit(ExitEventArgs e)
	{
		Mcl.Core.Utils.Logger.Default.Info("应用程序退出");
		base.OnExit(e);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "9.0.12.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			base.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
			Uri resourceLocator = new Uri("/WPFLauncher;component/app.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
	}

	[STAThread]
	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "9.0.12.0")]
	public static void Main()
	{
		App app = new App();
		app.InitializeComponent();
		app.Run();
	}
}
