using System;
using WPFLauncher.Model.Common;

namespace WPFLauncher.Modules.Login;

[Serializable]
public class ProtocolBase<EntityType> where EntityType : EntityBase
{
	public int code { get; set; }

	public string message { get; set; }

	public string details { get; set; }

	public EntityType entity { get; set; }

	protected ProtocolBase()
	{
	}
}
