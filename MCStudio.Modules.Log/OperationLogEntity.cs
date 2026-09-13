namespace MCStudio.Modules.Log;

internal class OperationLogEntity<T>
{
	public string type { get; set; }

	public T data { get; set; }
}
