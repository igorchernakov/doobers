using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using DG.Tweening;

public class Doober : MonoBehaviour 
{
	public struct Location
	{
		private Vector3 worldPosition;
		private Vector3 localPosition;
		private Transform transform;
		private Camera camera;

		public Location(Vector3 worldPosition, Transform transform)
		{
			this.worldPosition = worldPosition;
			this.localPosition = transform.InverseTransformPoint(worldPosition);
			this.transform = transform;
			this.camera = transform is RectTransform ? 
				transform.GetComponentInParent<Canvas>().worldCamera :
				Camera.main;
		}

		public Vector3 Project(Matrix4x4 m)
		{
			if (transform) {
				worldPosition = transform.TransformPoint(localPosition);
			}
			return camera.ConvertPointToCamera(m, worldPosition);
		}
	}

	private Location _a, _b;
	private Vector3 curve_a, curve_b;
	private Matrix4x4 _dobcam;
	private float _time, _total;
	private Action _callback;

	public Image image;

	public AnimationCurve easing;
	public AnimationCurve elevation;

	public float scaleUpTime = 0.2f;
	public float scaleDownTime = 0.1f;
	public Vector2 curveValue;

	private const float MIN_TIME = 0.5f;

	private float Progress { 
		get { 
			return easing.Evaluate(Mathf.Clamp01(_time / _total)); 
		}
	}

	private Vector3 Curve { 
		get { 
			return Vector3.Lerp(
				curve_a * elevation.Evaluate(Progress),
				curve_b * elevation.Evaluate(Progress),
				Progress);
		}
	}

	public void ApplyTrajectory(Location a, Location b, Camera dobcam, float time, Action callback) 
	{
		_a = a; 
		_b = b;
		_dobcam = dobcam.GetInverseMVP();
		_time = 0;
		_total = time;
		_callback = callback;

		var screen_a = dobcam.WorldToScreenPoint(_a.Project(_dobcam));
		var screen_b = dobcam.WorldToScreenPoint(_b.Project(_dobcam));

		screen_a.Scale(new Vector3(1f / Screen.width, 1f / Screen.height));
		screen_b.Scale(new Vector3(1f / Screen.width, 1f / Screen.height));

		// apply a coefficient for curving depending on distance between points
		float k = (screen_a - screen_b).magnitude;

		curve_a = new Vector3(
			Mathf.Lerp(curveValue.x, -curveValue.x, screen_a.x),
			Mathf.Lerp(curveValue.y, -curveValue.y, screen_a.y)) * k;
		
		curve_b = new Vector3(
			Mathf.Lerp(curveValue.x, -curveValue.x, screen_b.x),
			Mathf.Lerp(-curveValue.y, curveValue.y, screen_b.y)) * k;

		_total *= Mathf.Lerp(MIN_TIME, 1f, k);

		UpdatePosition();
		StartCoroutine(Scaling(_total));
	}

	public IEnumerator Scaling(float time)
	{
		transform.localScale = Vector3.zero;
		transform.DOScale(Vector3.one, scaleUpTime);
		yield return new WaitForSeconds(time);
		if (_callback != null) _callback.Invoke();
		transform.DOScale(Vector3.zero, scaleDownTime);
		yield return new WaitForSeconds(scaleDownTime);
		Destroy(gameObject);
	}

	public void Update()
	{
		_time += Time.deltaTime;

		UpdatePosition();
	}

	private void UpdatePosition()
	{
		transform.position = Vector3.Lerp(_a.Project(_dobcam), _b.Project(_dobcam), Progress) + Curve;
	}
}
