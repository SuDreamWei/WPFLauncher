using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Mcl.Core.Network.Interface;
using Newtonsoft.Json;
using WPFLauncher.DataTypes;
using WPFLauncher.Network.Http;
using iNKORE.UI.WPF.Modern.Controls;

namespace WPFLauncher.Pages;

public class ItemList : iNKORE.UI.WPF.Modern.Controls.Page, INotifyPropertyChanged, IComponentConnector
{
	private ObservableCollection<ItemListModel> _allItems = new ObservableCollection<ItemListModel>();

	private ObservableCollection<ItemListModel> _displayItems = new ObservableCollection<ItemListModel>();

	private const int PageSize = 36;

	private int _currentPage = 1;

	private int _totalPages = 1;

	private string _pageInputText = "1";

	private int _totalItemsCount;

	internal TextBox TextBox_SearchText;

	internal CheckBox isPE_CheckBox;

	internal System.Windows.Controls.ListView ItemListView;

	internal Button PreviousPageButton;

	internal Button NextPageButton;

	private bool _contentLoaded;

	public int CurrentPage
	{
		get
		{
			return _currentPage;
		}
		set
		{
			_currentPage = value;
			OnPropertyChanged("CurrentPage");
			UpdatePageButtons();
		}
	}

	public int TotalPages
	{
		get
		{
			return _totalPages;
		}
		set
		{
			_totalPages = value;
			OnPropertyChanged("TotalPages");
			UpdatePageButtons();
		}
	}

	public string PageInputText
	{
		get
		{
			return _pageInputText;
		}
		set
		{
			_pageInputText = value;
			OnPropertyChanged("PageInputText");
		}
	}

	public ObservableCollection<ItemListModel> ItemListModels
	{
		get
		{
			return _displayItems;
		}
		set
		{
			_displayItems = value;
			OnPropertyChanged("ItemListModels");
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	public ItemList()
	{
		InitializeComponent();
		base.DataContext = this;
		_displayItems = new ObservableCollection<ItemListModel>();
		_allItems = new ObservableCollection<ItemListModel>();
	}

	private async void Search_OnClick(object sender, RoutedEventArgs e)
	{
		if (!Var.CurrentAccount.HasLogin)
		{
			Function.ShowDialog("请先登录账号");
			return;
		}
		CurrentPage = 1;
		await ExecuteSearch();
	}

	private async Task ExecuteSearch()
	{
		string text = TextBox_SearchText.Text?.Trim();
		if (string.IsNullOrWhiteSpace(text))
		{
			await SearchAvailableMods();
		}
		else
		{
			await SearchModsByKeyword(text);
		}
	}

	private async void PreviousPage_Click(object sender, RoutedEventArgs e)
	{
		if (CurrentPage > 1)
		{
			CurrentPage--;
			await ExecuteSearch();
		}
	}

	private async void NextPage_Click(object sender, RoutedEventArgs e)
	{
		if (CurrentPage < TotalPages)
		{
			CurrentPage++;
			await ExecuteSearch();
		}
	}

	private void LoadSampleData()
	{
		for (int i = 1; i <= 50; i++)
		{
			_allItems.Add(new ItemListModel
			{
				ItemName = $"模组 {i}",
				DeveloperName = $"开发者 {i}",
				ItemDescription = $"这是第 {i} 个模组的描述信息"
			});
		}
		CalculateTotalPages();
		GoToPage(1);
		TestPaginationFormat();
	}

	private void TestPaginationFormat()
	{
		for (int i = 1; i <= 3; i++)
		{
			GetPaginationParameters(i);
		}
		TestApiRequestFormat();
	}

	private void TestApiRequestFormat()
	{
		JsonConvert.SerializeObject(new QueryAvailableRequest
		{
			offset = 0,
			length = 36
		}, Formatting.None);
		JsonConvert.SerializeObject(new SearchByKeywordRequest
		{
			keyword = "测试",
			offset = 0,
			length = 36
		}, Formatting.None);
		JsonConvert.SerializeObject(new SearchByIdsRequest
		{
			entity_ids = new List<string> { "123", "456" }
		}, Formatting.None);
		JsonConvert.SerializeObject(new ModItemModel
		{
			entity_id = "test123",
			name = "测试模组",
			developer_name = "测试开发者",
			brief_summary = "这是一个测试模组"
		}, Formatting.None);
	}

	private void LoadAllItems(Dictionary<string, int> paginationParams)
	{
		CalculateTotalPages();
		GoToPage(1);
	}

	private void FilterItems(string keyword, Dictionary<string, int> paginationParams)
	{
		CalculateTotalPages();
		GoToPage(1);
	}

	private void CalculateTotalPages()
	{
		TotalPages = (_totalItemsCount + 36 - 1) / 36;
		if (TotalPages == 0)
		{
			TotalPages = 1;
		}
	}

	private async void GoToPage(int pageNumber)
	{
		if (pageNumber >= 1 && pageNumber <= TotalPages)
		{
			CurrentPage = pageNumber;
			PageInputText = pageNumber.ToString();
			await ExecuteSearch();
		}
	}

	private void UpdatePageButtons()
	{
		if (PreviousPageButton != null)
		{
			PreviousPageButton.IsEnabled = CurrentPage > 1;
		}
		if (NextPageButton != null)
		{
			NextPageButton.IsEnabled = CurrentPage < TotalPages;
		}
	}

	public Dictionary<string, int> GetPaginationParameters()
	{
		return new Dictionary<string, int>
		{
			{ "length", 36 },
			{
				"offset",
				(CurrentPage - 1) * 36
			}
		};
	}

	public Dictionary<string, int> GetPaginationParameters(int pageNumber)
	{
		return new Dictionary<string, int>
		{
			{ "length", 36 },
			{
				"offset",
				(pageNumber - 1) * 36
			}
		};
	}

	private async Task SearchAvailableMods()
	{
		try
		{
			Dictionary<string, int> paginationParameters = GetPaginationParameters();
			QueryAvailableRequest value = new QueryAvailableRequest
			{
				offset = paginationParameters["offset"],
				length = paginationParameters["length"]
			};
			if (!isPE_CheckBox.IsChecked.Value)
			{
				string parameter = JsonConvert.SerializeObject(value);
				INetResponse netResponse = X19Http.Post("/item/query/available", parameter);
				if (netResponse.StatusCode == HttpStatusCode.OK)
				{
					ApiResponse<ModItemModel> apiResponse = JsonConvert.DeserializeObject<ApiResponse<ModItemModel>>(netResponse.Content);
					if (apiResponse.Code == 0 && apiResponse.Entities != null)
					{
						_totalItemsCount = apiResponse.Total;
						CalculateTotalPages();
						List<string> list = apiResponse.Entities.Select((ModItemModel e) => e.entity_id).ToList();
						if (list.Any())
						{
							await GetModDetailsByIds(list);
							return;
						}
						_displayItems.Clear();
						Function.ShowDialog("未找到可用模组");
					}
					else
					{
						Function.ShowDialog("获取模组列表失败: " + (apiResponse?.Message ?? "未知错误"));
					}
				}
				else
				{
					Function.ShowDialog($"网络请求失败: {netResponse.StatusCode}");
				}
				return;
			}
			string parameter2 = $"{{\"channel_id\": 5, \"length\": {paginationParameters["length"]}, \"first_type\": \"2\", \"is_unofficial\": true, \"offset\": {paginationParameters["offset"]}, \"mod_second_type\": \"0\", \"asc_flag\": false, \"sort_type\": 2, \"filter_type\": 0}}";
			INetResponse netResponse2 = X19Http.Post("/pe-item/query/search-by-type/", parameter2, ProtocolOption.Normal, "https://g79apigatewayobt.minecraft.cn/");
			if (netResponse2.StatusCode == HttpStatusCode.OK)
			{
				ApiResponse<ModItemModel> apiResponse2 = JsonConvert.DeserializeObject<ApiResponse<ModItemModel>>(netResponse2.Content);
				if (apiResponse2.Code == 0 && apiResponse2.Entities != null)
				{
					_totalItemsCount = apiResponse2.Total;
					CalculateTotalPages();
					if (apiResponse2.Entities.Select((ModItemModel e) => e.entity_id).ToList().Any())
					{
						_displayItems.Clear();
						{
							foreach (ModItemModel entity in apiResponse2.Entities)
							{
								ItemListModel item = new ItemListModel
								{
									ItemName = entity.res_name,
									DeveloperName = entity.developer_name,
									ItemDescription = $"[钻石数: {entity.diamonds}, 绿宝石数: {entity.points}]暂不支持查看详细信息,组件码: {entity.normal_number}",
									ItemImage = entity.title_image_url,
									ItemId = entity.entity_id
								};
								_displayItems.Add(item);
							}
							return;
						}
					}
					_displayItems.Clear();
					Function.ShowDialog("未找到可用模组");
				}
				else
				{
					Function.ShowDialog("获取模组列表失败: " + (apiResponse2?.Message ?? "未知错误"));
				}
			}
			else
			{
				Function.ShowDialog($"网络请求失败: {netResponse2.StatusCode}");
			}
		}
		catch (Exception ex)
		{
			Function.ShowDialog("搜索失败: " + ex.Message);
		}
	}

	private async Task SearchModsByKeyword(string keyword)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(keyword))
			{
				Function.ShowDialog("搜索关键词不能为空");
				return;
			}
			Dictionary<string, int> paginationParameters = GetPaginationParameters();
			SearchByKeywordRequest value = new SearchByKeywordRequest
			{
				keyword = keyword,
				offset = paginationParameters["offset"],
				length = paginationParameters["length"]
			};
			if (!isPE_CheckBox.IsChecked.Value)
			{
				string parameter = JsonConvert.SerializeObject(value, Formatting.None);
				INetResponse netResponse = X19Http.Post("/item/query/search-by-keyword", parameter);
				if (netResponse == null)
				{
					Function.ShowDialog("网络请求返回空响应");
				}
				else if (netResponse.StatusCode == HttpStatusCode.OK)
				{
					if (string.IsNullOrEmpty(netResponse.Content))
					{
						Function.ShowDialog("服务器返回空内容");
						return;
					}
					ApiResponse<ModItemModel> apiResponse = JsonConvert.DeserializeObject<ApiResponse<ModItemModel>>(netResponse.Content);
					if (apiResponse == null)
					{
						Function.ShowDialog("解析响应内容失败");
					}
					else if (apiResponse.Code == 0)
					{
						if (apiResponse.Entities == null)
						{
							apiResponse.Entities = new List<ModItemModel>();
						}
						_totalItemsCount = apiResponse.Total;
						CalculateTotalPages();
						List<string> list = (from e in apiResponse.Entities
							select e.entity_id into id
							where !string.IsNullOrEmpty(id)
							select id).ToList();
						if (list.Any())
						{
							await GetModDetailsByIds(list);
							return;
						}
						_displayItems.Clear();
						Function.ShowDialog("未找到包含 '" + keyword + "' 的模组");
					}
					else
					{
						Function.ShowDialog($"搜索失败: {apiResponse.Message ?? "未知错误"} (Code: {apiResponse.Code})");
					}
				}
				else
				{
					Function.ShowDialog($"网络请求失败: {netResponse.StatusCode} ({(int)netResponse.StatusCode})");
				}
				return;
			}
			string parameter2 = $"{{\"second_type\": [], \"official_skin\": 0, \"filter_domain_server_item\": 0, \"sort_type\": 0, \"first_type\": 0, \"offset\": {paginationParameters["offset"]}, \"keyword\": \"{keyword}\", \"price_type\": 0, \"init\": 0, \"channel_id\": 5, \"length\": {paginationParameters["length"]}}}";
			INetResponse netResponse2 = X19Http.Post("/pe-item/query/search-by-keyword/", parameter2, ProtocolOption.Normal, "https://g79apigatewayobt.minecraft.cn/");
			if (netResponse2 == null)
			{
				Function.ShowDialog("网络请求返回空响应");
			}
			else if (netResponse2.StatusCode == HttpStatusCode.OK)
			{
				if (string.IsNullOrEmpty(netResponse2.Content))
				{
					Function.ShowDialog("服务器返回空内容");
					return;
				}
				ApiResponse<ModItemModel> apiResponse2 = JsonConvert.DeserializeObject<ApiResponse<ModItemModel>>(netResponse2.Content);
				if (apiResponse2 == null)
				{
					Function.ShowDialog("解析响应内容失败");
				}
				else if (apiResponse2.Code == 0)
				{
					if (apiResponse2.Entities == null)
					{
						apiResponse2.Entities = new List<ModItemModel>();
					}
					_totalItemsCount = apiResponse2.Total;
					CalculateTotalPages();
					if ((from e in apiResponse2.Entities
						select e.entity_id into id
						where !string.IsNullOrEmpty(id)
						select id).ToList().Any())
					{
						_displayItems.Clear();
						{
							foreach (ModItemModel entity in apiResponse2.Entities)
							{
								ItemListModel item = new ItemListModel
								{
									ItemName = entity.res_name,
									DeveloperName = entity.developer_name,
									ItemDescription = $"[钻石数: {entity.diamonds}, 绿宝石数: {entity.points}]暂不支持查看详细信息,组件码: {entity.normal_number}",
									ItemImage = entity.title_image_url,
									ItemId = entity.entity_id
								};
								_displayItems.Add(item);
							}
							return;
						}
					}
					_displayItems.Clear();
					Function.ShowDialog("未找到包含 '" + keyword + "' 的模组");
				}
				else
				{
					Function.ShowDialog($"搜索失败: {apiResponse2.Message ?? "未知错误"} (Code: {apiResponse2.Code})");
				}
			}
			else
			{
				Function.ShowDialog($"网络请求失败: {netResponse2.StatusCode} ({(int)netResponse2.StatusCode})");
			}
		}
		catch (JsonException ex)
		{
			Function.ShowDialog("数据解析错误: " + ex.Message);
		}
		catch (WebException ex2)
		{
			Function.ShowDialog("网络连接错误: " + ex2.Message);
		}
		catch (Exception ex3)
		{
			Function.ShowDialog("搜索失败: " + ex3.Message);
		}
	}

	private async Task GetModDetailsByIds(List<string> entityIds)
	{
		try
		{
			SearchByIdsRequest value = new SearchByIdsRequest
			{
				entity_ids = entityIds
			};
			if (!isPE_CheckBox.IsChecked.Value)
			{
				string parameter = JsonConvert.SerializeObject(value);
				INetResponse netResponse = X19Http.Post("/item/query/search-by-ids", parameter);
				if (netResponse.StatusCode == HttpStatusCode.OK)
				{
					ApiResponse<ModItemModel> apiResponse = JsonConvert.DeserializeObject<ApiResponse<ModItemModel>>(netResponse.Content);
					if (apiResponse.Code == 0 && apiResponse.Entities != null)
					{
						_displayItems.Clear();
						{
							foreach (ModItemModel entity in apiResponse.Entities)
							{
								ItemListModel item = new ItemListModel
								{
									ItemName = entity.name,
									DeveloperName = entity.developer_name,
									ItemDescription = $"[钻石数: {entity.diamonds}, 绿宝石数: {entity.points}, 组件码: {entity.normal_number}]" + entity.brief_summary,
									ItemImage = entity.title_image_url,
									ItemId = entity.entity_id
								};
								_displayItems.Add(item);
							}
							return;
						}
					}
					Function.ShowDialog("获取模组详情失败: " + (apiResponse?.Message ?? "未知错误"));
				}
				else
				{
					Function.ShowDialog($"网络请求失败: {netResponse.StatusCode}");
				}
				return;
			}
			string parameter2 = JsonConvert.SerializeObject(value);
			INetResponse netResponse2 = X19Http.Post("/pe-item/query/search-by-ids", parameter2, ProtocolOption.Normal, "https://g79apigatewayobt.minecraft.cn/");
			if (netResponse2.StatusCode == HttpStatusCode.OK)
			{
				ApiResponse<ModItemModel> apiResponse2 = JsonConvert.DeserializeObject<ApiResponse<ModItemModel>>(netResponse2.Content);
				if (apiResponse2.Code == 0 && apiResponse2.Entities != null)
				{
					_displayItems.Clear();
					{
						foreach (ModItemModel entity2 in apiResponse2.Entities)
						{
							ItemListModel item2 = new ItemListModel
							{
								ItemName = entity2.name,
								DeveloperName = entity2.developer_name,
								ItemDescription = entity2.brief_summary,
								ItemImage = entity2.title_image_url,
								ItemId = entity2.entity_id
							};
							_displayItems.Add(item2);
						}
						return;
					}
				}
				Function.ShowDialog("获取模组详情失败: " + (apiResponse2?.Message ?? "未知错误"));
			}
			else
			{
				Function.ShowDialog($"网络请求失败: {netResponse2.StatusCode}");
			}
		}
		catch (Exception ex)
		{
			Function.ShowDialog("获取模组详情失败: " + ex.Message);
		}
	}

	protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	private void Download_Click(object sender, RoutedEventArgs e)
	{
		IList selectedItems = ItemListView.SelectedItems;
		if (selectedItems.Count == 0)
		{
			Function.ShowDialog("请选择模组");
			return;
		}
		List<ItemListModel> list = new List<ItemListModel>();
		foreach (ItemListModel item in selectedItems)
		{
			list.Add(item);
			Console.WriteLine("ItemId: " + item.ItemId + ", ItemName: " + item.ItemName);
		}
		new ItemDownloadProgress(list, isPE_CheckBox.IsChecked.Value).ShowAsync();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "9.0.12.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/WPFLauncher;component/pages/itemlist.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "9.0.12.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			TextBox_SearchText = (TextBox)target;
			break;
		case 2:
			isPE_CheckBox = (CheckBox)target;
			break;
		case 3:
			((Button)target).Click += Search_OnClick;
			break;
		case 4:
			ItemListView = (System.Windows.Controls.ListView)target;
			break;
		case 5:
			PreviousPageButton = (Button)target;
			PreviousPageButton.Click += PreviousPage_Click;
			break;
		case 6:
			NextPageButton = (Button)target;
			NextPageButton.Click += NextPage_Click;
			break;
		case 7:
			((Button)target).Click += Download_Click;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
