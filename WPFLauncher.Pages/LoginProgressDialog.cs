using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using WPFLauncher.DataTypes;
using WPFLauncher.ViewModels;
using iNKORE.UI.WPF.Controls;
using iNKORE.UI.WPF.Modern.Controls;

namespace WPFLauncher.Pages;

public class LoginProgressDialog : ContentDialog, IComponentConnector
{
	private readonly LoginProgressViewModel _viewModel;

	private readonly Account _account;

	private bool _isCancelled;

	private bool _loginCompleted;

	private string _captchaText = "";

	private bool _isInputCaptcha;

	private string Base64Image = "";

	internal SimpleStackPanel ProgressName;

	internal TextBlock CurrentStepText;

	internal System.Windows.Controls.ProgressBar LoginProgressBar;

	internal TextBlock ErrorText;

	internal TextBlock SuccessText;

	internal SimpleStackPanel CaptchaStackPanel;

	internal Image CaptchaImage;

	internal TextBox CaptchaTextBox;

	internal Button CaptchaButton;

	private bool _contentLoaded;

	public LoginProgressDialog(Account account)
	{
		InitializeComponent();
		_account = account;
		_viewModel = new LoginProgressViewModel();
		base.DataContext = _viewModel;
	}

	private async void LoginProgressDialog_OnLoaded(object sender, RoutedEventArgs e)
	{
		await StartLoginProcess();
	}

	private async Task<string> OnCaptchaRequested(string base64Image)
	{
		Base64Image = base64Image;
		CaptchaStackPanel.Visibility = Visibility.Visible;
		ProgressName.Visibility = Visibility.Collapsed;
		BitmapImage bitmapImage = new BitmapImage();
		bitmapImage.BeginInit();
		bitmapImage.StreamSource = new MemoryStream(Convert.FromBase64String(base64Image));
		bitmapImage.EndInit();
		CaptchaImage.Source = bitmapImage;
		await Task.Run(delegate
		{
			while (!_isInputCaptcha)
			{
				Thread.Sleep(100);
			}
		});
		CaptchaStackPanel.Visibility = Visibility.Collapsed;
		ProgressName.Visibility = Visibility.Visible;
		return _captchaText;
	}

	private void CaptchaButton_Click(object sender, RoutedEventArgs e)
	{
		_captchaText = CaptchaTextBox.Text.ToString();
		_isInputCaptcha = true;
		CaptchaButton.IsEnabled = false;
	}

	private async Task StartLoginProcess()
	{
		_viewModel.Reset();
		_viewModel.IsInProgress = true;
		_loginCompleted = false;
		try
		{
			_account.LoginSuccess += OnLoginSuccess;
			_account.LoginFailed += OnLoginFailed;
			_account.LoginProgressUpdated += OnLoginProgressUpdated;
			_account.Login(OnCaptchaRequested);
			Task timeoutTask = Task.Delay(30000);
			if (await Task.WhenAny(WaitForLoginCompletion(), timeoutTask) == timeoutTask)
			{
				_viewModel.ErrorMessage = "登录超时";
				_viewModel.IsInProgress = false;
			}
		}
		catch (Exception ex)
		{
			_viewModel.ErrorMessage = "登录过程中发生错误: " + ex.Message;
			_viewModel.IsInProgress = false;
		}
		finally
		{
			_account.LoginSuccess -= OnLoginSuccess;
			_account.LoginFailed -= OnLoginFailed;
			_account.LoginProgressUpdated -= OnLoginProgressUpdated;
		}
	}

	private void OnLoginProgressUpdated(string step)
	{
		if (_isCancelled)
		{
			return;
		}
		base.Dispatcher.Invoke(delegate
		{
			double num = step switch
			{
				"1/4 获取Sauth..." => 25.0, 
				"2/4 获取一次性token..." => 50.0, 
				"3/4 获取AuthenticationOtp..." => 75.0, 
				"4/4 获取用户信息完成" => 100.0, 
				_ => _viewModel.ProgressValue, 
			};
			_viewModel.UpdateProgress(step, num);
			if (num >= 100.0 && !_loginCompleted)
			{
				_loginCompleted = true;
				ShowLoginSuccess();
				Task.Delay(1500).ContinueWith(delegate
				{
					base.Dispatcher.Invoke(delegate
					{
						Hide();
					});
				});
			}
		});
	}

	private Task WaitForLoginCompletion()
	{
		TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
		bool isCompleted = false;
		_account.LoginSuccess += OnSuccess;
		_account.LoginFailed += OnFailed;
		if (_account.HasLogin)
		{
			OnSuccess(_account);
		}
		return tcs.Task;
		void OnFailed(Account acc, string error)
		{
			if (!_isCancelled && !isCompleted)
			{
				isCompleted = true;
				_viewModel.ErrorMessage = error;
				_viewModel.IsInProgress = false;
				tcs.TrySetResult(result: false);
			}
		}
		void OnSuccess(Account acc)
		{
			if (!_isCancelled && !isCompleted)
			{
				isCompleted = true;
				_viewModel.UpdateProgress("4/4 获取用户信息完成", 100.0);
				_viewModel.IsInProgress = false;
				tcs.TrySetResult(result: true);
			}
		}
	}

	private void OnLoginSuccess(Account account)
	{
		if (_isCancelled)
		{
			return;
		}
		base.Dispatcher.Invoke(delegate
		{
			_viewModel.UpdateProgress("4/4 获取用户信息完成", 100.0);
			_viewModel.IsInProgress = false;
			_loginCompleted = true;
			base.IsPrimaryButtonEnabled = true;
			ShowLoginSuccess();
			Task.Delay(1500).ContinueWith(delegate
			{
				base.Dispatcher.Invoke(delegate
				{
					Hide();
				});
			});
		});
	}

	private void OnLoginFailed(Account account, string error)
	{
		if (!_isCancelled)
		{
			base.Dispatcher.Invoke(delegate
			{
				_viewModel.ErrorMessage = error;
				_viewModel.IsInProgress = false;
				base.IsPrimaryButtonEnabled = true;
			});
		}
	}

	private void LoginProgressDialog_OnClosing(ContentDialog sender, ContentDialogClosingEventArgs args)
	{
		_isCancelled = true;
		_viewModel.IsInProgress = false;
	}

	private void OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
	{
		_isCancelled = false;
		_loginCompleted = false;
		SuccessText.Visibility = Visibility.Collapsed;
		_viewModel.ErrorMessage = "";
		StartLoginProcess();
	}

	private void ShowLoginSuccess()
	{
		SuccessText.Visibility = Visibility.Visible;
		Task.Run(delegate
		{
			Function.ShowDialog("登录成功！");
		});
	}

	private void OCRGetResultButton_Click(object sender, RoutedEventArgs e)
	{
		CaptchaTextBox.Text = Ocr.GetOcrResult(Base64Image);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "9.0.12.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/WPFLauncher;component/dialog/loginprogressdialog.xaml", UriKind.Relative);
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
			((LoginProgressDialog)target).PrimaryButtonClick += OnPrimaryButtonClick;
			((LoginProgressDialog)target).Loaded += LoginProgressDialog_OnLoaded;
			((LoginProgressDialog)target).Closing += LoginProgressDialog_OnClosing;
			break;
		case 2:
			ProgressName = (SimpleStackPanel)target;
			break;
		case 3:
			CurrentStepText = (TextBlock)target;
			break;
		case 4:
			LoginProgressBar = (System.Windows.Controls.ProgressBar)target;
			break;
		case 5:
			ErrorText = (TextBlock)target;
			break;
		case 6:
			SuccessText = (TextBlock)target;
			break;
		case 7:
			CaptchaStackPanel = (SimpleStackPanel)target;
			break;
		case 8:
			CaptchaImage = (Image)target;
			break;
		case 9:
			CaptchaTextBox = (TextBox)target;
			break;
		case 10:
			CaptchaButton = (Button)target;
			CaptchaButton.Click += CaptchaButton_Click;
			break;
		case 11:
			((Button)target).Click += OCRGetResultButton_Click;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
