using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public abstract class ResourceManager<T> : MonoBehaviour where T: Object
{
	private Dictionary<string, T> m_lookup;
	private static readonly Regex s_regex = new Regex(@"\.\d+$");

	public T fallback;
	public T[] resources;

	public T this [string key] {
		get {
			if (!IsInitialized)
				Initialize();
			T value;
			if (!m_lookup.TryGetValue(key, out value)) {
				if (s_regex.IsMatch(key)) { // if key ends with a number
					key = s_regex.Replace(key, ".0");
					if (m_lookup.TryGetValue(key, out value)) {
						return value;
					}
				}
			}
			return value ? value : fallback;
		}
		set {
			if (!IsInitialized)
				Initialize();
			if (m_lookup.ContainsKey(key)) {
				m_lookup[key] = value;
		}
	}
	}

	public bool HasItem(string key) {
		if (!IsInitialized)
			Initialize();
		return m_lookup.ContainsKey(key);
	}

	public bool TryGetValue(string key, out T value) {
		if (!IsInitialized)
			Initialize();
		if (!m_lookup.TryGetValue(key, out value)) {
			value = fallback;
			Debug.LogWarningFormat("Didn't find resource for key {0}.", key);
			return false;
		} else {
			return true;
		}
	}

	public T GetSimilar (string substring)
	{
		return GetSimilar(substring, true);
	}

	public T GetSimilar (string substring, bool allowFallback)
	{
		if (!IsInitialized) {
			Initialize();
		}

		foreach (var kvp in m_lookup) {
			if (kvp.Key.Contains(substring)) {
				return kvp.Value;
			}
		}

		if (allowFallback) {
			Debug.LogWarningFormat("Didn't find resource for substring {0}.", substring);
			return fallback;
		} else {
			return null;
		}
	}

	private void Initialize() {
		m_lookup = new Dictionary<string, T>(System.StringComparer.OrdinalIgnoreCase);
		foreach (T element in resources) {
			if (!element)
				continue;
			m_lookup[element.name] = element;
		}
	}

	private bool IsInitialized {
		get {
			return m_lookup != null;
		}
	}

	public T GetRandom() {
		if (resources.Length <= 0)
			return default(T);
		return resources[Random.Range(0, resources.Length)];
	}
}

public interface IConvertibleTo<T>
{
	void Convert (out T result);
}

public abstract class ResourceManager<T1, T2> 
	: ResourceManager<T1> 
	where T2: IConvertibleTo<T1> 
	where T1: Object
{
	public abstract void Convert (out T1 result);
}
