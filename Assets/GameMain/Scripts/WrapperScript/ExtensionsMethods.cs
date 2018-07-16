using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class ExtensionsMethods
{
	public static bool NearEquals(this float a, float b, int decimalPlaces = 4)
	{
		float single = 1f / Mathf.Pow(10f, (float)decimalPlaces);
		return Math.Abs(a - b) < single;
	}
}