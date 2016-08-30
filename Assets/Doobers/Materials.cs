using System;

public struct MaterialAmount
{
	public string type;
	public int amount;

	public MaterialAmount (string name, int value)
	{
		type = name;
		amount = value;
	}
}
