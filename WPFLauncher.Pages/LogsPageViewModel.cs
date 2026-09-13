using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WPFLauncher.Pages;

public class LogsPageViewModel : INotifyPropertyChanged
{
	private string _currentState = "空闲";

	private string _currentTask = "无任务";

	private bool _isIndeterminate;

	private bool _isInfoBarOpen = true;

	private string _logs = "";

	private string _message = "无消息";

	private double _progressValue;

	public string CurrentTask
	{
		get
		{
			return _currentTask;
		}
		set
		{
			_currentTask = value;
			OnPropertyChanged("CurrentTask");
		}
	}

	public string CurrentState
	{
		get
		{
			return _currentState;
		}
		set
		{
			_currentState = value;
			OnPropertyChanged("CurrentState");
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

	public bool IsIndeterminate
	{
		get
		{
			return _isIndeterminate;
		}
		set
		{
			_isIndeterminate = value;
			OnPropertyChanged("IsIndeterminate");
		}
	}

	public string Logs
	{
		get
		{
			return _logs;
		}
		set
		{
			_logs = value;
			OnPropertyChanged("Logs");
		}
	}

	public string Message
	{
		get
		{
			return _message;
		}
		set
		{
			_message = value;
			OnPropertyChanged("Message");
		}
	}

	public bool IsInfoBarOpen
	{
		get
		{
			return _isInfoBarOpen;
		}
		set
		{
			_isInfoBarOpen = value;
			OnPropertyChanged("IsInfoBarOpen");
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	public void AddLog(string message)
	{
		string value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
		Logs += $"[{value}] {message}\n";
	}

	public void ClearLogs()
	{
		Logs = "";
	}
}
