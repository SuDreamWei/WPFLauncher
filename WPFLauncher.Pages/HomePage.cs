using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using iNKORE.UI.WPF.Modern.Controls;

namespace WPFLauncher.Pages;

public class HomePage : iNKORE.UI.WPF.Modern.Controls.Page, IComponentConnector
{
	internal TextBlock Notify;

	internal HyperlinkButton Control1;

	private bool _contentLoaded;

	public HomePage()
	{
		InitializeComponent();
		base.DataContext = this;
		Notify.Text = "测试版本(不接入公告API)";
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "9.0.12.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/WPFLauncher;component/pages/homepage.xaml", UriKind.Relative);
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
			Notify = (TextBlock)target;
			break;
		case 2:
			Control1 = (HyperlinkButton)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
