using System.Collections.Generic;

namespace WPFLauncher.DataTypes;

public class SearchByIdsRequest
{
	public List<string> entity_ids { get; set; }

	public int with_price { get; set; } = 1;

	public int with_title_image { get; set; } = 1;

	public string channel_id { get; set; } = "11";

	public bool is_has { get; set; } = true;
}
