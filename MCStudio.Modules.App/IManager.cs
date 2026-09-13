namespace MCStudio.Modules.App;

public interface IManager
{
	void Initial();

	void LoginSuccess();

	void Logout();

	void Cleanup();
}
