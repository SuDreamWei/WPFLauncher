using Mcl.Core.Utils;

namespace MCStudio.Modules.App;

public class ManagerBase<T> : Singleton<T>, IManager where T : class, new()
{
	public virtual void Initial()
	{
	}

	public virtual void LoginSuccess()
	{
	}

	public virtual void Logout()
	{
	}

	public virtual void Cleanup()
	{
		Logout();
	}
}
