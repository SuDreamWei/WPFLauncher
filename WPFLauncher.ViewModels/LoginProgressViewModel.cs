using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WPFLauncher.ViewModels;

public class LoginProgressViewModel : INotifyPropertyChanged
{
	private string _currentStep = "准备登录...";

	private double _progressValue;

	private bool _isInProgress;

	private string _errorMessage = "";

	public string CurrentStep
	{
		get
		{
			return _currentStep;
		}
		set
		{
			_currentStep = value;
			OnPropertyChanged("CurrentStep");
		}
	}

	public double ProgressValue
	{
		get
		{
			return _progressValue;
		}
		set
		{
			_progressValue = value;
			OnPropertyChanged("ProgressValue");
		}
	}

	public bool IsInProgress
	{
		get
		{
			return _isInProgress;
		}
		set
		{
			_isInProgress = value;
			OnPropertyChanged("IsInProgress");
		}
	}

	public string ErrorMessage
	{
		get
		{
			return _errorMessage;
		}
		set
		{
			_errorMessage = value;
			OnPropertyChanged("ErrorMessage");
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	public void Reset()
	{
		CurrentStep = "准备登录...";
		ProgressValue = 0.0;
		IsInProgress = false;
		ErrorMessage = "";
	}

	public void UpdateProgress(string step, double progress)
	{
		CurrentStep = step;
		ProgressValue = progress;
	}

	protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
