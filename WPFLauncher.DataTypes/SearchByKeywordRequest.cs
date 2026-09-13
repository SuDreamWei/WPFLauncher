namespace WPFLauncher.DataTypes;

public class SearchByKeywordRequest
{
	public int item_type { get; set; } = 2;

	public string keyword { get; set; }

	public string master_type_id { get; set; } = "0";

	public string secondary_type_id { get; set; } = "0";

	public int sort_type { get; set; } = 2;

	public int order { get; set; }

	public int offset { get; set; }

	public int length { get; set; }

	public bool is_has { get; set; } = true;

	public int year { get; set; }

	public int is_sync { get; set; }

	public int price_type { get; set; }
}
