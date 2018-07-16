using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Utilities
{
	internal static class ReflectionUtilities
	{
		public static object Construct(Type type, params object[] values)
		{
			ConstructorInfo constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, (
				from v in (IEnumerable<object>)values
				select v.GetType()).ToArray<Type>(), null);
			return constructor.Invoke(values);
		}

		public static T Construct<T>(params object[] values)
		{
			return (T)ReflectionUtilities.Construct(typeof(T), values);
		}

		public static T GetCustomAttribute<T>(this MemberInfo memberInfo, bool inherit = true)
		where T : Attribute
		{
			return Attribute.GetCustomAttributes(memberInfo, typeof(T), inherit).Cast<T>().First<T>();
		}

		internal static IEnumerable<KeyValuePair<PropertyInfo, DataColumnAttribute>> GetDataColumnProperties(Type type)
		{
			//ReflectionUtilities.< GetDataColumnProperties > c__Iterator0 variable = null;
			//return variable;

			if ((type != typeof(DataEntry)) && !type.IsSubclassOf(typeof(DataEntry)))
			{
				throw new ArgumentException(string.Format("Type {0} is not subclass of {1}", type.Name, typeof(DataEntry).Name));
			}
			//FixIt
			yield return new KeyValuePair<PropertyInfo, DataColumnAttribute>();



		}



		public static IEnumerable<KeyValuePair<PropertyInfo, DataColumnAttribute>> GetDataColumnProperties<T>()
		where T : DataEntry
		{
			return ReflectionUtilities.GetDataColumnProperties(typeof(T));
		}

		private static object GetDefault(Type type)
		{
			return typeof(ReflectionUtilities).GetMethod("GetDefaultGeneric", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(new Type[] { type }).Invoke(null, null);
		}

		private static T GetDefaultGeneric<T>()
		{
			return default(T);
		}

		public static int GetPrimaryKey<T>(this T entry)
		where T : DataEntry
		{
			PropertyInfo primaryKeyInfo = ReflectionUtilities.GetPrimaryKeyInfo<T>();
			if (primaryKeyInfo.PropertyType != typeof(int))
			{
				throw new InvalidOperationException(string.Format("Primary key of table {0} is type of {1} (should be int)", ReflectionUtilities.GetTableName<T>(), primaryKeyInfo.PropertyType.Name));
			}
			return (int)primaryKeyInfo.GetValue(entry, null);
		}

		public static PropertyInfo GetPrimaryKeyInfo<T>()
		where T : DataEntry
		{
			Type type = typeof(T);
			if (!ReflectionUtilities.GetDataColumnProperties(type).Any<KeyValuePair<PropertyInfo, DataColumnAttribute>>((KeyValuePair<PropertyInfo, DataColumnAttribute> kvp) => kvp.Value.IsPrimaryKey))
			{
				throw new InvalidOperationException(string.Format("Table {0} doesn't have primary key", ReflectionUtilities.GetTableName<T>()));
			}
			KeyValuePair<PropertyInfo, DataColumnAttribute> keyValuePair = ReflectionUtilities.GetDataColumnProperties(type).First<KeyValuePair<PropertyInfo, DataColumnAttribute>>((KeyValuePair<PropertyInfo, DataColumnAttribute> kvp) => kvp.Value.IsPrimaryKey);
			return keyValuePair.Key;
		}

		internal static string GetTableName(Type type)
		{
			if (type != typeof(DataEntry) && !type.IsSubclassOf(typeof(DataEntry)))
			{
				throw new ArgumentException(string.Format("Type {0} is not subclass of {1}", type.Name, typeof(DataEntry).Name));
			}
			DataTableAttribute dataTableAttribute = type.GetCustomAttributes(typeof(DataTableAttribute), false).Cast<DataTableAttribute>().FirstOrDefault<DataTableAttribute>();
			if (dataTableAttribute == null)
			{
				throw new InvalidOperationException(string.Format("Data table attribute not found for type: {0}", type.Name));
			}
			string name = dataTableAttribute.Name;
			if (string.IsNullOrEmpty(name))
			{
				throw new InvalidOperationException(string.Format("Data table name is empty for type: {0}", type.Name));
			}
			return name;
		}

		public static string GetTableName<T>()
		where T : DataEntry
		{
			return ReflectionUtilities.GetTableName(typeof(T));
		}

		public static string GetValue<T>(this T entry, PropertyInfo property)
		where T : DataEntry
		{
			DataColumnAttribute customAttribute;
			Type propertyType = property.PropertyType;
			object value = property.GetValue(entry, null);
			if (!property.HasCustomAttribute<DataColumnAttribute>(true))
			{
				customAttribute = null;
			}
			else
			{
				customAttribute = property.GetCustomAttribute<DataColumnAttribute>(true);
			}
			DataColumnAttribute dataColumnAttribute = customAttribute;
			if (value == null && dataColumnAttribute != null && dataColumnAttribute.DefaultValue != null)
			{
				value = dataColumnAttribute.DefaultValue;
				propertyType = value.GetType();
			}
			return ReflectionUtilities.GetValue(propertyType, value);
		}

		public static string GetValue(Type type, object value)
		{
			if (type == typeof(int) || type == typeof(short) || type == typeof(long))
			{
				return value.ToString();
			}
			if (type == typeof(bool))
			{
				return (!(bool)value ? "0" : "1");
			}
			if (type == typeof(string))
			{
				return (string)value;
			}
			if (type == typeof(DateTime))
			{
				return ((DateTime)value).ToString("yyyy-MM-dd HH':'mm':'ss");
			}
			if (type == typeof(float) || type == typeof(double))
			{
				return value.ToString();
			}
			if (!type.IsSubclassOf(typeof(DataEntry)))
			{
				throw new NotSupportedException(string.Format("Type {0} is not supported in DB", type));
			}
			return (value == null ? string.Empty : ((DataEntry)value).GetPrimaryKey<DataEntry>().ToString());
		}

		public static bool HasCustomAttribute<T>(this MemberInfo memberInfo, bool inherit = true)
		where T : Attribute
		{
			return Attribute.GetCustomAttributes(memberInfo, typeof(T), inherit).Cast<T>().Any<T>();
		}

		public static bool IsPrimaryKeyAutoincrement<T>()
		where T : DataEntry
		{
			Type type = typeof(T);
			if (!ReflectionUtilities.GetDataColumnProperties(type).Any<KeyValuePair<PropertyInfo, DataColumnAttribute>>((KeyValuePair<PropertyInfo, DataColumnAttribute> kvp) => kvp.Value.IsPrimaryKey))
			{
				throw new InvalidOperationException(string.Format("Table {0} doesn't have primary key", ReflectionUtilities.GetTableName<T>()));
			}
			KeyValuePair<PropertyInfo, DataColumnAttribute> keyValuePair = ReflectionUtilities.GetDataColumnProperties(type).First<KeyValuePair<PropertyInfo, DataColumnAttribute>>((KeyValuePair<PropertyInfo, DataColumnAttribute> kvp) => kvp.Value.IsPrimaryKey);
			return keyValuePair.Value.IsAutoincrement;
		}

		public static void SetValue<T>(this T entry, PropertyInfo property, string value)
		where T : DataEntry
		{
			entry.SetValue(property, value);
		}

		public static void SetValue(this object entry, PropertyInfo property, string value)
		{
			bool flag;
			DataColumnAttribute customAttribute;
			if (!entry.GetType().IsSubclassOf(typeof(DataEntry)))
			{
				throw new ArgumentException(string.Format("Type {0} is not subclass of {1}", entry.GetType().Name, typeof(DataEntry).Name));
			}
			if (!property.CanWrite)
			{
				return;
			}
			Type propertyType = property.PropertyType;
			if (string.IsNullOrEmpty(value))
			{
				if (!property.HasCustomAttribute<DataColumnAttribute>(true))
				{
					customAttribute = null;
				}
				else
				{
					customAttribute = property.GetCustomAttribute<DataColumnAttribute>(true);
				}
				DataColumnAttribute dataColumnAttribute = customAttribute;
				if (dataColumnAttribute != null && dataColumnAttribute.DefaultValue != null && dataColumnAttribute.DefaultValue.GetType() == propertyType)
				{
					property.SetValue(entry, dataColumnAttribute.DefaultValue, null);
				}
				else if (propertyType != typeof(float))
				{
					property.SetValue(entry, ReflectionUtilities.GetDefault(propertyType), null);
				}
				else
				{
					property.SetValue(entry, -1f, null);
				}
				return;
			}
			if (propertyType == typeof(int))
			{
				property.SetValue(entry, value.ParseInt(), null);
			}
			else if (propertyType == typeof(short))
			{
				property.SetValue(entry, value.ParseShort(), null);
			}
			else if (propertyType == typeof(long))
			{
				property.SetValue(entry, value.ParseLong(), null);
			}
			else if (propertyType == typeof(bool))
			{
				if (value != null)
				{
					if (value == "1")
					{
						flag = true;
						goto Label0;
					}
					else
					{
						if (value != "0")
						{
							goto Label2;
						}
						flag = false;
						goto Label0;
					}
				}
				Label2:
				flag = value.ParseBool();
				Label0:
				property.SetValue(entry, flag, null);
			}
			else if (propertyType == typeof(string))
			{
				property.SetValue(entry, value, null);
			}
			else if (propertyType == typeof(DateTime))
			{
				property.SetValue(entry, value.ParseDateTime(), null);
			}
			else if (propertyType == typeof(float))
			{
				property.SetValue(entry, value.ParseFloat(), null);
			}
			else if (propertyType != typeof(double))
			{
				if (!propertyType.IsSubclassOf(typeof(DataEntry)))
				{
					throw new NotSupportedException(string.Format("Type {0} is not supported in DB", propertyType));
				}
				property.SetValue(entry, DataEntry.GetInstance(propertyType, value.ParseInt()), null);
			}
			else
			{
				property.SetValue(entry, value.ParseDouble(), null);
			}
		}
	}
}