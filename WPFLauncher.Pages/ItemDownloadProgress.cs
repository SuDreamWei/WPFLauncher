using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Mcl.Core.Network.Interface;
using Mcl.Core.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WPFLauncher.DataTypes;
using WPFLauncher.Network.Http;
using iNKORE.UI.WPF.Controls;
using iNKORE.UI.WPF.Modern.Controls;

namespace WPFLauncher.Pages;

public class ItemDownloadProgress : ContentDialog, IComponentConnector
{
	public List<ItemListModel> ItemList = new List<ItemListModel>();

	public bool IsPE;

	internal SimpleStackPanel ProgressName;

	internal TextBlock CurrentStepText;

	internal iNKORE.UI.WPF.Modern.Controls.ProgressBar DownloadProgressBar;

	internal TextBlock ErrorText;

	internal TextBlock SuccessText;

	private bool _contentLoaded;

	public ItemDownloadProgress(List<ItemListModel> itemList, bool isPE)
	{
		InitializeComponent();
		ItemList = itemList;
		IsPE = isPE;
	}

	private void UpdateProgress(int value, string message)
	{
		DownloadProgressBar.Value = value;
		CurrentStepText.Text = message;
	}

	private void ErrProgress(string reason)
	{
		DownloadProgressBar.ShowError = true;
		CurrentStepText.Text += " - Failed";
		ErrorText.Text = reason;
		ErrorText.Visibility = Visibility.Visible;
		Function.AddLog(reason);
	}

	private void ItemDownloadProgress_OnLoaded(object sender, RoutedEventArgs e)
	{
		try
		{
			List<string> itemIdList = new List<string>();
			List<ItemDownloadModel> itemDownloadModelList = new List<ItemDownloadModel>();
			Task.Run(async delegate
			{
				try
				{
					foreach (ItemListModel item in ItemList)
					{
						itemIdList.Add(item.ItemId);
					}
					Application.Current.Dispatcher.Invoke(delegate
					{
						UpdateProgress(0, "正在获取模组下载链接");
					});
					INetResponse getDownloadUrlResponse = X19Http.Post("/pe-item/query/search-lobby-by-id-list", JsonConvert.SerializeObject(new
					{
						item_ids = itemIdList
					}));
					if (getDownloadUrlResponse.StatusCode == HttpStatusCode.OK)
					{
						Application.Current.Dispatcher.Invoke(delegate
						{
							UpdateProgress(17, "正在获取模组解密密钥");
						});
						string device_id = "";
						INetResponse getEncryptKeyResponseData = X19Http.Post("/pe-item/get-encryption-key-list-for-guests", JsonConvert.SerializeObject(new
						{
							device_id = device_id,
							item_ids = itemIdList
						}), ProtocolOption.CommonEncrypt, "https://x19obtcore.nie.netease.com:8443");
						if (getEncryptKeyResponseData.StatusCode == HttpStatusCode.OK)
						{
							foreach (JToken jsonItem in (IEnumerable<JToken>)JObject.Parse(getEncryptKeyResponseData.Content)["entities"])
							{
								string[] array = jsonItem["jwt"].ToString().Split('.');
								if (array.Length > 1)
								{
									string text = array[1];
									int num = 4 - text.Length % 4;
									if (num != 4)
									{
										text += new string('=', num);
									}
									JObject jObject = JObject.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(text)));
									string decryptionKey = Function.GetDecryptionKey(device_id, jObject["contentKey"].ToString(), Var.CurrentAccount.UserID);
									foreach (JToken item2 in (IEnumerable<JToken>)JObject.Parse(getDownloadUrlResponse.Content)["entities"])
									{
										if (item2["iid"].ToString() == jsonItem["item_id"].ToString())
										{
											string text2 = string.Join("_", item2["res_name"].ToString().Split(Path.GetInvalidFileNameChars()));
											itemDownloadModelList.Add(new ItemDownloadModel
											{
												ItemId = item2["iid"].ToString(),
												DownloadPath = Path.Combine(Var.CurrentPath, "[" + text2 + "]" + Path.GetFileNameWithoutExtension(item2["lobby_res_url"].ToString())),
												Key = decryptionKey,
												Url = item2["lobby_res_url"].ToString(),
												UUID = jObject["contentUuid"].ToString(),
												ItemName = item2["res_name"].ToString()
											});
											break;
										}
									}
								}
								else
								{
									Application.Current.Dispatcher.Invoke(delegate
									{
										ErrProgress($"无法获取到模组密钥: {jsonItem["item_id"]}");
									});
								}
							}
							Application.Current.Dispatcher.Invoke(delegate
							{
								UpdateProgress(50, "正在下载模组");
							});
							int count = ItemList.Count;
							double downloadProgressPerItem = 25.0 / (double)count;
							double extractProgressPerItem = 25.0 / (double)count;
							int completedItems = 0;
							foreach (ItemDownloadModel itemDownloadModel in itemDownloadModelList)
							{
								await Function.DownloadFile(itemDownloadModel.Url, Path.Combine(Var.CurrentPath, Path.GetFileName(itemDownloadModel.Url)), delegate(int progress)
								{
									Application.Current.Dispatcher.Invoke(delegate
									{
										double num3 = 50.0 + (double)completedItems * downloadProgressPerItem + (double)progress * downloadProgressPerItem / 100.0;
										UpdateProgress((int)num3, "正在下载模组 - " + itemDownloadModel.ItemName);
									});
								});
								Function.ExtractZip(Path.Combine(Var.CurrentPath, Path.GetFileName(itemDownloadModel.Url)), itemDownloadModel.DownloadPath, delegate(int progress)
								{
									Application.Current.Dispatcher.Invoke(delegate
									{
										double num3 = 75.0 + (double)completedItems * extractProgressPerItem + (double)progress * extractProgressPerItem / 100.0;
										UpdateProgress((int)num3, "正在解压模组 - " + itemDownloadModel.ItemName);
									});
								});
								Application.Current.Dispatcher.Invoke(delegate
								{
									UpdateProgress((int)DownloadProgressBar.Value, "正在解密模组 - " + itemDownloadModel.ItemName);
								});
								Function.UnzipModJsonPath(itemDownloadModel.DownloadPath, itemDownloadModel.Key, itemDownloadModel.UUID);
								Application.Current.Dispatcher.Invoke(delegate
								{
									UpdateProgress((int)DownloadProgressBar.Value, "[完成]解密模组 - " + itemDownloadModel.ItemName);
								});
								Function.DeleteFile(Path.Combine(Var.CurrentPath, Path.GetFileName(itemDownloadModel.Url)));
								int num2 = completedItems;
								completedItems = num2 + 1;
							}
							Application.Current.Dispatcher.Invoke(delegate
							{
								UpdateProgress(100, "DONE!");
								base.IsPrimaryButtonEnabled = true;
							});
						}
						else
						{
							Application.Current.Dispatcher.Invoke(delegate
							{
								ErrProgress(getEncryptKeyResponseData.Content);
								base.IsPrimaryButtonEnabled = true;
							});
						}
					}
					else
					{
						Application.Current.Dispatcher.Invoke(delegate
						{
							ErrProgress(getDownloadUrlResponse.Content);
						});
						base.IsPrimaryButtonEnabled = true;
					}
				}
				catch (Exception ex3)
				{
					Exception ex4 = ex3;
					Exception exception2 = ex4;
					Console.WriteLine(exception2);
					Application.Current.Dispatcher.Invoke(delegate
					{
						ErrProgress(exception2.Message);
						base.IsPrimaryButtonEnabled = true;
					});
				}
			});
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			Exception exception = ex2;
			Logger.Default?.Error(exception, "LoginProgressDialog.xaml.cs");
			Application.Current.Dispatcher.Invoke(delegate
			{
				ErrProgress(exception.Message);
				base.IsPrimaryButtonEnabled = true;
			});
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "9.0.12.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/WPFLauncher;component/dialog/itemdownloadprogress.xaml", UriKind.Relative);
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
			((ItemDownloadProgress)target).Loaded += ItemDownloadProgress_OnLoaded;
			break;
		case 2:
			ProgressName = (SimpleStackPanel)target;
			break;
		case 3:
			CurrentStepText = (TextBlock)target;
			break;
		case 4:
			DownloadProgressBar = (iNKORE.UI.WPF.Modern.Controls.ProgressBar)target;
			break;
		case 5:
			ErrorText = (TextBlock)target;
			break;
		case 6:
			SuccessText = (TextBlock)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
