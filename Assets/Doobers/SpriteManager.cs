using UnityEngine;
using System.Collections.Generic;

public class SpriteManager : ResourceManager<Sprite>
{
}

public abstract class ACountSpriteManager : SpriteManager
{
	protected Dictionary<int,string> mapCountToString;

	public abstract int MaxCount{ get; }

	public abstract string Prefix{ get; }

	public virtual Sprite GetSprite (int count)
	{
		if (mapCountToString == null) {
			CreateCountMap();
		}

		return this[mapCountToString[count]];
	}

	protected virtual void CreateCountMap ()
	{
		mapCountToString = new Dictionary<int, string>();
		for (int i = 1; i <= MaxCount; ++i) {
			mapCountToString[i] = string.Format(Prefix, i);
		}
	}
}

public abstract class SpriteManager<T> : SpriteManager
{
	protected Dictionary<T,string> mapEnumToString;

	public Sprite GetSprite (T type)
	{
		if (mapEnumToString == null) {
			CreateEnumMap();
		}

		return this[mapEnumToString[type]];
	}

	protected abstract void CreateEnumMap ();
}