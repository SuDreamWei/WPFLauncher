using System;

namespace WPFLauncher.Model.Common;

[Serializable]
public class EntityResponse<EntityType> : ResponseBase where EntityType : EntityBase
{
	public EntityType entity { get; set; }
}
