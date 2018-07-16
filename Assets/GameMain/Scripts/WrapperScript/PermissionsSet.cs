using SkyTrakWrapper;
using System;
using System.Collections;

public class PermissionsSet
{
	private BitArray _flags;

	public PermissionsSet()
	{
	}

	public bool GetPermission(STSWMMSFeatureFlagType flag)
	{
		return this._flags.Get((int)flag);
	}

	public void InitializeByBitArray(BitArray featureFlags)
	{
		if (featureFlags.Length != this._flags.Length)
		{
			this._flags = new BitArray(featureFlags.Length);
		}
		for (int i = 0; i < featureFlags.Length; i++)
		{
			this._flags.Set(i, featureFlags.Get(i));
		}
	}

	public void InitializeByMask(int[] mask, int size)
	{
		this._flags = PermissionsSet.BitArrayConverter.ToBitArray(mask, size);
	}

	public void InitializeDefaults()
	{
		this._flags = new BitArray(256);
	}

	private static class BitArrayConverter
	{
		public static BitArray ToBitArray(int[] numeral, int size)
		{
			BitArray bitArrays = new BitArray(numeral);
			BitArray bitArrays1 = new BitArray(size);
			for (int i = 0; i != size; i++)
			{
				bitArrays1.Set(i, bitArrays.Get(i));
			}
			return bitArrays1;
		}
	}
}