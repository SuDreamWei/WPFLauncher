using System.Collections.Generic;

namespace WPFLauncher.DataTypes;

public class QueryAvailableRequest
{
	public int item_type { get; set; } = 2;

	public string master_type_id { get; set; } = "3";

	public string secondary_type_id { get; set; } = "0";

	public int sort_type { get; set; } = 2;

	public int order { get; set; }

	public int offset { get; set; }

	public int length { get; set; }

	public bool is_has { get; set; } = true;

	public int year { get; set; }

	public int is_sync { get; set; }

	public int price_type { get; set; }

	public List<string> available_mc_versions { get; set; } = new List<string> { "13" };
}
