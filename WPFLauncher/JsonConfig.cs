using Newtonsoft.Json;

namespace WPFLauncher;

public class JsonConfig
{
	[JsonProperty("BedrockPath")]
	public string bedrockPath;

	[JsonProperty("Channel")]
	public string channel;

	[JsonProperty("BedrockVersion")]
	public string selectedBedrockVersion;
}
