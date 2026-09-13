using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Microsoft.Win32;
using Newtonsoft.Json;
using iNKORE.UI.WPF.Modern.Controls;

namespace WPFLauncher.Pages;

public class SettingsPage : iNKORE.UI.WPF.Modern.Controls.Page, IComponentConnector
{
	public static readonly string ConfigFolder = Path.Combine(Var.CurrentPath, "config");

	private ObservableCollection<Tuple<string, string>> _canSelectBedrockVersion = new ObservableCollection<Tuple<string, string>>();

	public ObservableCollection<Tuple<string, string>> bedrockList = new ObservableCollection<Tuple<string, string>>
	{
		new Tuple<string, string>("网易版", "MCLauncher"),
		new Tuple<string, string>("4399版", "PC4399_MCLauncher"),
		new Tuple<string, string>("自定义路径", "Custom")
	};

	private string _bedrockPath = string.Empty;

	public static string pBedrockPath = string.Empty;

	public static string selectBedrockFolder = string.Empty;

	public static bool deleteDuplicate = false;

	internal ComboBox ComboBoxSelectBedrockPath;

	internal TextBox TextBoxSelectBedrockPath;

	internal ComboBox ComboBoxSelectBedrockVersion;

	internal SettingsCard SettingsCardCustomBedrockPath;

	private bool _contentLoaded;

	public ObservableCollection<Tuple<string, string>> canSelectBedrockVersion
	{
		get
		{
			return _canSelectBedrockVersion;
		}
		set
		{
			_canSelectBedrockVersion = value;
			OnPropertyChanged("canSelectBedrockVersion");
		}
	}

	public string bedrockPath
	{
		get
		{
			return _bedrockPath;
		}
		set
		{
			_bedrockPath = value;
			pBedrockPath = value;
			TextBoxSelectBedrockPath.Text = value;
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	public SettingsPage()
	{
		InitializeComponent();
		base.DataContext = this;
		foreach (Tuple<string, string> bedrock in bedrockList)
		{
			if (bedrock.Item2.Equals("Custom"))
			{
				canSelectBedrockVersion.Add(bedrock);
				continue;
			}
			using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Netease\\" + bedrock.Item2))
			{
				if (registryKey != null)
				{
					object value = registryKey.GetValue("MinecraftBENeteasePath");
					if (value != null)
					{
						canSelectBedrockVersion.Add(new Tuple<string, string>(bedrock.Item1, value.ToString()));
					}
				}
			}
			ComboBoxSelectBedrockPath.SelectedIndex = 0;
		}
		InitConfig();
	}

	private void SelectBedrockPath_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		string item = (ComboBoxSelectBedrockPath.SelectedItem as Tuple<string, string>).Item2;
		if (item.Equals("Custom"))
		{
			SettingsCardCustomBedrockPath.IsEnabled = true;
			return;
		}
		bedrockPath = item;
		GetMinecraftVersions(item);
		SettingsCardCustomBedrockPath.IsEnabled = false;
	}

	private void SelectFolder_OnClick(object sender, RoutedEventArgs e)
	{
		OpenFolderDialog openFolderDialog = new OpenFolderDialog
		{
			Title = "选择基岩版文件夹"
		};
		if (openFolderDialog.ShowDialog() == true)
		{
			string folderPath = (bedrockPath = openFolderDialog.FolderName);
			GetMinecraftVersions(folderPath);
		}
	}

	private void SaveConfig_Onclick(object sender, RoutedEventArgs e)
	{
		if (ComboBoxSelectBedrockVersion.SelectedIndex == -1)
		{
			Function.ShowDialog("你尚未选择基岩版版本", "错误");
			return;
		}
		if (!Directory.Exists(ConfigFolder))
		{
			Directory.CreateDirectory(ConfigFolder);
		}
		string text = ComboBoxSelectBedrockVersion.SelectedItem.ToString();
		File.WriteAllText(ConfigFolder + "\\settings.json", JsonConvert.SerializeObject(new JsonConfig
		{
			bedrockPath = bedrockPath,
			selectedBedrockVersion = text,
			channel = (ComboBoxSelectBedrockPath.SelectedItem as Tuple<string, string>).Item1
		}));
		GetMinecraftVersions(bedrockPath);
		selectBedrockFolder = text;
		Function.ShowDialog("保存设置成功!\n - 你选择的基岩版路径: " + Path.Combine(bedrockPath, text));
	}

	protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	public static bool VerifyMinecraftWindowsFolder(string folderPath)
	{
		if (!Directory.Exists(folderPath))
		{
			return false;
		}
		if (!File.Exists(Path.Combine(folderPath, "Minecraft.Windows.exe")))
		{
			return false;
		}
		if (!Directory.Exists(Path.Combine(folderPath, "data", "resource_packs", "vanilla_netease")))
		{
			return false;
		}
		return true;
	}

	public void GetMinecraftVersions(string folderPath)
	{
		string[] directories = Directory.GetDirectories(folderPath);
		List<string> list = new List<string>();
		string[] array = directories;
		foreach (string obj in array)
		{
			bool flag = VerifyMinecraftWindowsFolder(obj);
			string fileName = Path.GetFileName(obj);
			if (flag)
			{
				list.Add(fileName);
			}
		}
		ComboBoxSelectBedrockVersion.ItemsSource = list;
	}

	public static long GetCurrentTimestamp()
	{
		return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
	}

	public JsonConfig ReadConfig()
	{
		string value = File.ReadAllText(Path.Combine(ConfigFolder, "settings.json"));
		try
		{
			JsonConfig jJsonConfig = JsonConvert.DeserializeObject<JsonConfig>(value);
			if (!VerifyMinecraftWindowsFolder(Path.Combine(jJsonConfig.bedrockPath, jJsonConfig.selectedBedrockVersion)))
			{
				throw new Exception("目录校验失败");
			}
			if (!bedrockList.Any((Tuple<string, string> x) => x.Item1.Equals(jJsonConfig.channel)))
			{
				throw new Exception("渠道校验失败");
			}
			if (jJsonConfig.channel != "自定义路径")
			{
				string item = bedrockList.FirstOrDefault((Tuple<string, string> x) => x.Item1.Equals(jJsonConfig.channel)).Item2;
				if (item == null)
				{
					throw new Exception("渠道校验失败");
				}
				using RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Netease\\" + item);
				if (registryKey != null)
				{
					object value2 = registryKey.GetValue("MinecraftBENeteasePath");
					if (value2 != null)
					{
						jJsonConfig.bedrockPath = value2.ToString();
					}
					else
					{
						jJsonConfig.channel = "custom";
					}
				}
				else
				{
					jJsonConfig.channel = "custom";
				}
			}
			return JsonConvert.DeserializeObject<JsonConfig>(value);
		}
		catch (Exception ex)
		{
			string value3 = "settings.json_" + GetCurrentTimestamp() + ".bak";
			File.Copy(Path.Combine(ConfigFolder, "settings.json"), Path.Combine(ConfigFolder, "settings.json_" + GetCurrentTimestamp() + ".bak"));
			File.Delete(Path.Combine(ConfigFolder, "settings.json"));
			Function.ShowDialog($"我们在处理你的json文件时发生错误(我们已将原设置备份并且重新初始化了设置配置,文件名:{value3} in config folder):{ex.Message}\n StackTrace: \n{ex.StackTrace}", "错误");
			return null;
		}
	}

	public void InitConfig()
	{
		if (!File.Exists(Path.Combine(ConfigFolder, "settings.json")))
		{
			if (!Directory.Exists(ConfigFolder))
			{
				Directory.CreateDirectory(ConfigFolder);
			}
			return;
		}
		JsonConfig jsonConfig = ReadConfig();
		if (jsonConfig != null)
		{
			ComboBoxSelectBedrockPath.SelectedItem = jsonConfig.channel;
			bedrockPath = jsonConfig.bedrockPath;
			GetMinecraftVersions(bedrockPath);
			ComboBoxSelectBedrockVersion.SelectedItem = jsonConfig.selectedBedrockVersion;
			selectBedrockFolder = jsonConfig.selectedBedrockVersion;
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "9.0.12.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/WPFLauncher;component/pages/settingspage.xaml", UriKind.Relative);
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
			ComboBoxSelectBedrockPath = (ComboBox)target;
			ComboBoxSelectBedrockPath.SelectionChanged += SelectBedrockPath_OnSelectionChanged;
			break;
		case 2:
			TextBoxSelectBedrockPath = (TextBox)target;
			break;
		case 3:
			ComboBoxSelectBedrockVersion = (ComboBox)target;
			break;
		case 4:
			SettingsCardCustomBedrockPath = (SettingsCard)target;
			break;
		case 5:
			((Button)target).Click += SelectFolder_OnClick;
			break;
		case 6:
			((Button)target).Click += SaveConfig_Onclick;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
