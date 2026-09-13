using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Threading;
using Mcl.Core.Utils;

namespace MCStudio.Modules.App;

public class ThreadManager : ManagerBase<ThreadManager>
{
	private bool m_isUnitTest;

	public Thread MainThread;

	private ConcurrentDictionary<Task, CancellationTokenSource> m_taskTokenDict = new ConcurrentDictionary<Task, CancellationTokenSource>();

	public int MainThreadId { get; set; }

	public override void Logout()
	{
		Cancel();
	}

	public override void Cleanup()
	{
		Cancel();
	}

	public void MainThreadInitial(bool isTest = false)
	{
		m_isUnitTest = isTest;
		MainThread = Thread.CurrentThread;
		MainThreadId = MainThread.ManagedThreadId;
		DispatcherHelper.Initialize();
	}

	public bool IsMainThread()
	{
		return MainThreadId == Thread.CurrentThread.ManagedThreadId;
	}

	public void Cancel()
	{
		foreach (Task key in m_taskTokenDict.Keys)
		{
			if (!key.IsCompleted)
			{
				Cancel(key);
			}
		}
		m_taskTokenDict.Clear();
	}

	public void Cancel(Task task)
	{
		if (m_taskTokenDict.ContainsKey(task))
		{
			m_taskTokenDict[task]?.Cancel();
		}
	}

	public Task Create(Action method)
	{
		CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
		Task task = Task.Factory.StartNew(delegate
		{
			try
			{
				method?.Invoke();
			}
			catch (Exception err)
			{
				Logger.Default?.Error(err, "ThreadManager.Create 执行任务时发生异常");
			}
		}, cancellationTokenSource.Token, TaskCreationOptions.None, TaskScheduler.Default).ContinueWith(delegate(Task t)
		{
			m_taskTokenDict.TryRemove(t, out CancellationTokenSource _);
		});
		m_taskTokenDict[task] = cancellationTokenSource;
		return task;
	}

	public void CallInMainThread(Action method)
	{
		if (m_isUnitTest)
		{
			method?.Invoke();
		}
		else if (method != null)
		{
			DispatcherHelper.CheckBeginInvokeOnUI(method);
		}
	}

	public void CallInMainThreadSync(Action method)
	{
		if (m_isUnitTest)
		{
			method?.Invoke();
		}
		else if (IsMainThread())
		{
			method?.Invoke();
		}
		else if (method != null)
		{
			DispatcherHelper.UIDispatcher.Invoke(method);
		}
	}

	public void CallInNonMainThread(Action method)
	{
		if (m_isUnitTest)
		{
			method?.Invoke();
		}
		else if (IsMainThread())
		{
			Create(delegate
			{
				method?.Invoke();
			});
		}
		else
		{
			method?.Invoke();
		}
	}

	public void CallInNonMainThreadSync(Action method)
	{
		if (m_isUnitTest)
		{
			if (method != null)
			{
				method();
			}
			return;
		}
		if (!IsMainThread())
		{
			if (method != null)
			{
				method();
			}
			return;
		}
		DispatcherFrame frame = new DispatcherFrame();
		Task.Factory.StartNew(delegate
		{
			try
			{
				method?.Invoke();
			}
			catch (Exception err)
			{
				Logger.Default?.Error(err, "CallInNonMainThreadSync 执行任务时发生异常");
			}
		}, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).ContinueWith(delegate
		{
			frame.Continue = false;
		}, TaskContinuationOptions.NotOnCanceled);
		Dispatcher.PushFrame(frame);
	}
}
