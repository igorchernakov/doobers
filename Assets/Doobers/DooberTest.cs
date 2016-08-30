using UnityEngine;
using System.Collections;

public class DooberTest : MonoBehaviour 
{
	public Transform _from, _to;

	public string type;
	public int howMany;

	public void Spawn()
	{
		var root = GetComponentInParent<DoobersRoot>();
		root.SpawnDoobers(new MaterialAmount(type, howMany), _from, _to);
	}
}
