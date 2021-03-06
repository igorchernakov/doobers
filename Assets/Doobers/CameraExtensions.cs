﻿using UnityEngine;
using System.Collections;

public enum ObjectLayer
{
	DebugInfo = 8,
	GridInfo = 9,
}

public static class CameraExtensions 
{
	// TODO: see if this is faster than WorldToScreenPoint() when using IL2CPP 
	public static Vector3 ConvertPointToCamera(this Camera self, Camera target, Vector3 position)
	{
		var m1 = target.GetInverseMVP();
		return self.ConvertPointToCamera(m1, position);

//		Vector3 screenPos = self.WorldToScreenPoint(position);
//		Vector3 worldPos = target.ScreenToWorldPoint(screenPos);
//		float z = Mathf.InverseLerp(self.nearClipPlane, self.farClipPlane, screenPos.z);
//		return new Vector3(worldPos.x, worldPos.y, Mathf.Lerp(target.nearClipPlane, target.farClipPlane, screenPos.z));
	}

	public static Vector3 ConvertPointToCamera(this Camera self, Matrix4x4 inv_mvp, Vector3 position)
	{
		return inv_mvp.MultiplyPoint(self.GetMVP().MultiplyPoint(position));
	}

	public static Matrix4x4 GetMVP(this Camera self)
	{
		return self.projectionMatrix * self.worldToCameraMatrix;
	}

	public static Matrix4x4 GetInverseMVP(this Camera self)
	{
		return self.cameraToWorldMatrix * self.projectionMatrix.inverse;
	}

	public static bool IsLayerVisible(this Camera self, ObjectLayer layer)
	{
		int mask = 1 << (int)layer;
		return (self.cullingMask & mask) == mask;
	}

	public static void SetLayerVisible(this Camera self, ObjectLayer layer, bool visible)
	{
		int mask = 1 << (int)layer;
		if (!visible) {
			self.cullingMask &= ~mask;
		} else {
			self.cullingMask |= mask;
		}
	}
}
