using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class DooberDesc
{
	public string name;
	public int unitsPerInstance = 100;
	public Doober doober;
}

public class DoobersRoot : MonoBehaviour
{
	public DooberDesc[] doobers;
	public float spawnSpeed = 1f;
	public float maxSpawnPeriod = 0.25f;
	public float flyTime = 2;
	public Canvas doobersCanvas;
	public SpriteManager[] imageSources;

	protected void Awake ()
	{
		foreach (var doober in GetComponentsInChildren<Doober>()) {
			doober.gameObject.SetActive(false);
		}
	}

	public float SpawnDoobers(MaterialAmount mat, Transform a, Transform b, System.Action<bool> callback = null) 
	{
		var dd = FindDesc(mat.type);

		if (dd == null)
			dd = FindDesc("default");

		if (dd == null) {
			Debug.LogWarningFormat("Could not find a setup for doober type {0}", mat.type);
			return 0;
		}

		StartCoroutine(Spawn(mat.type, dd, 
			Mathf.CeilToInt((float)mat.amount / (float)dd.unitsPerInstance), a, b, callback));

		return flyTime;
	}

	private IEnumerator Spawn(string name, DooberDesc dd, int count, Transform a, Transform b, 
		System.Action<bool> callback)
	{
		var wait = new WaitForSeconds(Mathf.Min(maxSpawnPeriod, spawnSpeed / count));

		Sprite image = null;

		foreach (var manager in imageSources) {
			image = manager.GetSimilar(name, false);
			if (image) break;
		}

		if (image) {
			System.Action first = () => callback.Invoke(true);
			System.Action second = () => callback.Invoke(false);
			for (int i = 0; i < count; i++) {
				var d = Instantiate<Doober>(dd.doober);
				d.transform.SetParent(doobersCanvas.transform, false);
				d.gameObject.SetActive(true);
				d.image.sprite = image;
				Vector3 va = a is RectTransform ? RandomWithin(a as RectTransform) : RandomWithin(a);
				Vector3 vb = b is RectTransform ? Exact(b as RectTransform) : RandomWithin(b);
				d.ApplyTrajectory(new Doober.Location(va, a), new Doober.Location(vb, b), 
					doobersCanvas.worldCamera, flyTime, callback != null ? (i == 0 ? first : second) : null);
				yield return wait;
			}
		} else {
			Debug.LogWarningFormat("Could not find an image for doober type {0}", name);
		}
	}

	private Vector3 Exact(Transform transform)
	{
		return transform.position;
	}

	private Vector3 RandomWithin(Transform rt)
	{
		var col = rt.GetComponent<Collider>();
		var pos = rt.position;
		if (col) {
			var b = col.bounds;
			pos = new Vector3(
				Random.Range(b.min.x, b.max.x), 
				Random.Range(b.min.y, b.max.y), 
				Random.Range(b.min.z, b.max.z));
		} 
		return pos;
	}

	private Vector3 RandomWithin(RectTransform rt)
	{
		var x = Random.Range(rt.rect.xMin, rt.rect.xMax);
		var y = Random.Range(rt.rect.yMin, rt.rect.yMax);
		return rt.TransformPoint(new Vector3(x, y));
	}

	private Vector3 Exact(RectTransform rt)
	{
		var x = (rt.rect.xMin + rt.rect.xMax) * 0.5f;
		var y = (rt.rect.yMin + rt.rect.yMax) * 0.5f;
		return rt.TransformPoint(new Vector3(x, y));
	}

	private DooberDesc FindDesc(string name)
	{
		return System.Array.Find(doobers, d => d.name == name);
	}
}
