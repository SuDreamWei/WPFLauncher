using System.Collections.Generic;

namespace WPFLauncher.DataTypes;

public class ModItemModel
{
	public string entity_id { get; set; }

	public string name { get; set; }

	public string developer_name { get; set; }

	public string res_name { get; set; }

	public string brief_summary { get; set; }

	public string master_type_id { get; set; }

	public string secondary_type_id { get; set; }

	public int item_type { get; set; }

	public int download_num { get; set; }

	public int like_num { get; set; }

	public long publish_time { get; set; }

	public int review_status { get; set; }

	public bool is_auth { get; set; }

	public bool is_has { get; set; }

	public string title_image_url { get; set; }

	public int points { get; set; }

	public int diamonds { get; set; }

	public int discount { get; set; }

	public string normal_number { get; set; }

	public string vanity_number { get; set; }

	public bool vip_only { get; set; }

	public int vip_discount { get; set; }

	public string network_tag { get; set; }

	public int rarity { get; set; }

	public int balance_grade { get; set; }

	public int available_scope { get; set; }

	public int game_status { get; set; }

	public int goods_state { get; set; }

	public int is_apollo { get; set; }

	public int is_refunding { get; set; }

	public string item_version { get; set; }

	public int lobby_max_num { get; set; }

	public int lobby_min_num { get; set; }

	public int mod_id { get; set; }

	public long rel_iid { get; set; }

	public int resource_version { get; set; }

	public int season_begin { get; set; }

	public int season_number { get; set; }

	public long tExpire { get; set; }

	public List<object> dyeing_list { get; set; }

	public string dyeing_origin_iid { get; set; }

	public int status { get; set; }

	public string summary_md5 { get; set; }

	public long expire_time { get; set; }

	public int effect_mtypeid { get; set; }

	public int effect_stypeid { get; set; }

	public bool is_current_season { get; set; }
}
