using System.Collections.Generic;

namespace WPFLauncher.DataTypes;

public class ApiResponse<T>
{
	public int Code { get; set; }

	public string Message { get; set; }

	public string Details { get; set; }

	public List<T> Entities { get; set; }

	public int Total { get; set; }

	public string SummaryMd5 { get; set; }
}
