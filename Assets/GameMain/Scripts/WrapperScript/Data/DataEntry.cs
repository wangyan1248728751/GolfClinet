using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using Utilities;

namespace Data
{
	public abstract class DataEntry
	{
		private const string ColumnsFieldJsonName = "COLUMNS";

		private const string DataFieldJsonName = "DATA";

		private readonly static Dictionary<Type, Dictionary<int, object>> _dataEntriesPool;

		[DataColumn("id", true, false, null)]
		public virtual int Id
		{
			get;
			protected set;
		}

		[DataColumn("isDeleted", false, false, null)]
		[Synchronizable]
		public bool IsDeleted
		{
			get;
			set;
		}

		[DataColumn("isDirty", false, false, null)]
		public bool IsDirty
		{
			get;
			set;
		}

		[DataColumn("isOnServer", false, false, null)]
		public bool IsOnServer
		{
			get;
			set;
		}

		[DataColumn("isReadyToBeSynchronized", false, false, null)]
		public bool IsReadyToBeSynchronized
		{
			get;
			set;
		}

		[DataColumn("isSynchronized", false, false, null)]
		public bool IsSynchronized
		{
			get;
			set;
		}

		static DataEntry()
		{
			DataEntry._dataEntriesPool = new Dictionary<Type, Dictionary<int, object>>();
		}

		protected DataEntry(int id)
		{
			this.Id = id;
			this.IsDirty = true;
			this.IsSynchronized = false;
			this.IsOnServer = false;
			this.IsDeleted = false;
			this.IsReadyToBeSynchronized = false;
			Type type = this.GetType();
			if (!DataEntry._dataEntriesPool.ContainsKey(type))
			{
				DataEntry._dataEntriesPool[type] = new Dictionary<int, object>();
			}
			if (DataEntry._dataEntriesPool[type].ContainsKey(id))
			{
				throw new DataEntryException(string.Format("{0} with ID {1} is already created", type.Name, id));
			}
			DataEntry._dataEntriesPool[type][id] = this;
		}

		//public static IEnumerable<T> DeserializeFromColumnsAndDataJson<T>(JSONObject json)
		//where T : DataEntry
		//{
		//	DataEntry.< DeserializeFromColumnsAndDataJson > c__Iterator0 < T > variable = null;
		//	return variable;
		//}

		public static void DeserializeFromColumnsAndDataJsonAsync<T>(JsonTextReader reader, Action<T> objectCallback, Action onFinish)
		where T : DataEntry
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (objectCallback == null)
			{
				throw new ArgumentNullException("objectCallback");
			}
			if (!reader.Read())
			{
				throw new JsonException("JSON is finished unexpectedly");
			}
			if (reader.TokenType != JsonToken.StartObject)
			{
				throw new JsonException(string.Format("Couldn't find {0} token", JsonToken.StartObject));
			}
			DataEntry.DataEntryColumn[] synchronizableColumns = DataEntry.GetSynchronizableColumns<T>();
			if (!reader.Read())
			{
				throw new JsonException("JSON is finished unexpectedly");
			}
			if (reader.TokenType == JsonToken.PropertyName)
			{
				if ((reader.Value as string ?? string.Empty).ToUpperInvariant() == "COLUMNS")
				{
					if (!reader.Read())
					{
						throw new JsonException("JSON is finished unexpectedly");
					}
					string[] strArrays = DataEntry.ParseStringsArray(reader);
					DataEntry.DataEntryColumn dataEntryColumn = synchronizableColumns.FirstOrDefault<DataEntry.DataEntryColumn>((DataEntry.DataEntryColumn c) => (c.SynchronizableAttribute == null || c.DataColumnAttribute == null ? false : c.DataColumnAttribute.IsPrimaryKey));
					Dictionary<int, int> nums = new Dictionary<int, int>();
					int num = -1;
					for (int i = 0; i < (int)strArrays.Length; i++)
					{
						int num1 = 0;
						while (num1 < (int)synchronizableColumns.Length)
						{
							if (string.Equals(strArrays[i], synchronizableColumns[num1].SynchronizableName, StringComparison.InvariantCultureIgnoreCase))
							{
								if (synchronizableColumns[num1].DataColumnAttribute != null && synchronizableColumns[num1].DataColumnAttribute.IsPrimaryKey)
								{
									num = i;
								}
								nums.Add(i, num1);
								break;
							}
							else
							{
								num1++;
							}
						}
					}
					//bool flag = (num != -1 ? false : ReflectionUtilities.IsPrimaryKeyAutoincrement<T>());
					bool flag = true;
					if (dataEntryColumn == null && !flag)
					{
						throw new InvalidOperationException(string.Format("Couldn't find a synchronizable primary key for type {0}", typeof(T).Name));
					}
					if (num == -1 && !flag)
					{
						throw new InvalidOperationException(string.Format("Couldn't find a primary key field in JSON for type {0}", typeof(T).Name));
					}
					if (!reader.Read())
					{
						throw new JsonException("JSON is finished unexpectedly");
					}
					if (reader.TokenType == JsonToken.PropertyName)
					{
						if ((reader.Value as string ?? string.Empty).ToUpperInvariant() == "DATA")
						{
							if (!reader.Read())
							{
								throw new JsonException("JSON is finished unexpectedly");
							}
							if (reader.TokenType != JsonToken.StartArray)
							{
								throw new JsonException(string.Format("Token {0} expected", JsonToken.StartArray));
							}
							if (!reader.Read())
							{
								throw new JsonException("JSON is finished unexpectedly");
							}
							while (reader.TokenType != JsonToken.EndArray)
							{
								string[] strArrays1 = DataEntry.ParseStringsArray(reader);
								int num2 = (!flag ? strArrays1[num].ParseInt() : DataEntry.GetAvailableId<T>());
								T instance = DataEntry.GetInstance<T>(num2);
								for (int j = 0; j < (int)strArrays.Length; j++)
								{
									if (j != num && nums.ContainsKey(j))
									{
										if (synchronizableColumns[nums[j]].SynchronizableAttribute.Pull)
										{
											try
											{
												synchronizableColumns[nums[j]].SetValue(instance, strArrays1[j]);
											}
											catch (FormatException formatException1)
											{
												FormatException formatException = formatException1;
												throw new SynchronizationException(string.Format("Failed to fill synchronizable property '{0}' with value '{1}' for {2} with ID {3}. Exception: {4}", new object[] { strArrays[j], strArrays1[j], typeof(T).Name, num2, formatException }));
											}
										}
									}
								}
								objectCallback(instance);
								if (reader.Read())
								{
									continue;
								}
								throw new JsonException("JSON is finished unexpectedly");
							}
							if (onFinish != null)
							{
								onFinish();
							}
							return;
						}
					}
					throw new JsonException(string.Format("Token {0} not found", "DATA"));
				}
			}
			throw new JsonException(string.Format("Token {0} not found", "COLUMNS"));
		}

		public static T DeserializeFromData<T>(JSONObject json)
		where T : DataEntry
		{
			DataEntry.DataEntryColumn[] synchronizableColumns = DataEntry.GetSynchronizableColumns<T>();
			string synchronizableName = synchronizableColumns.First<DataEntry.DataEntryColumn>((DataEntry.DataEntryColumn column) => column.DataColumnAttribute.IsPrimaryKey).SynchronizableName;
			int num = json[synchronizableName].str.ParseInt();
			T instance = DataEntry.GetInstance<T>(num);
			DataEntry.DataEntryColumn[] dataEntryColumnArray = synchronizableColumns;
			for (int i = 0; i < (int)dataEntryColumnArray.Length; i++)
			{
				DataEntry.DataEntryColumn dataEntryColumn = dataEntryColumnArray[i];
				if (dataEntryColumn.SynchronizableAttribute.Pull && json.HasField(dataEntryColumn.SynchronizableName))
				{
					try
					{
						dataEntryColumn.SetValue(instance, json[dataEntryColumn.SynchronizableName].str);
					}
					catch (FormatException formatException1)
					{
						FormatException formatException = formatException1;
						UnityEngine.Debug.Log("Failed to fill synchronizable property");
						throw new SynchronizationException(string.Format("Failed to fill synchronizable property '{0}' with value '{1}' for {2} with ID {3}. Exception: {4}", new object[] { dataEntryColumn.SynchronizableName, json[dataEntryColumn.SynchronizableName], ReflectionUtilities.GetTableName<T>(), num, formatException }));
					}
				}
			}
			return instance;
		}

		private static int GetAvailableId<T>()
		where T : DataEntry
		{
			//int availableId = DatabaseManager.GetAvailableId<T>();
			//while (DataEntry.InstanceExists<T>(availableId))
			//{
			//	availableId++;
			//}
			int availableId = 0;
			return availableId;
		}

		public static T GetInstance<T>(int id)
		where T : DataEntry
		{
			return (T)DataEntry.GetInstance(typeof(T), id);
		}

		public static object GetInstance(Type type, int id)
		{
			if (!type.IsSubclassOf(typeof(DataEntry)))
			{
				throw new ArgumentException(string.Format("Type {0} is not subclass of {1}", type.Name, typeof(DataEntry).Name));
			}
			if (!DataEntry._dataEntriesPool.ContainsKey(type))
			{
				DataEntry._dataEntriesPool[type] = new Dictionary<int, object>();
			}
			if (DataEntry._dataEntriesPool[type].ContainsKey(id))
			{
				return DataEntry._dataEntriesPool[type][id];
			}
			//object obj = DatabaseManager.Select(type, id);
			//if (obj != null)
			//{
			//	return obj;
			//}
			return ReflectionUtilities.Construct(type, new object[] { id });
		}

		private static DataEntry.DataEntryColumn[] GetSynchronizableColumns<T>()
		where T : DataEntry
		{
			DataEntry.DataEntryColumn[] array = ((IEnumerable<PropertyInfo>)typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)).Select<PropertyInfo, DataEntry.DataEntryColumn>(new Func<PropertyInfo, DataEntry.DataEntryColumn>(DataEntry.DataEntryColumn.Create)).Where<DataEntry.DataEntryColumn>((DataEntry.DataEntryColumn c) => (c == null ? false : c.SynchronizableAttribute != null)).ToArray<DataEntry.DataEntryColumn>();
			if (array.Any<DataEntry.DataEntryColumn>((DataEntry.DataEntryColumn c) => string.IsNullOrEmpty(c.SynchronizableName)))
			{
				throw new InvalidOperationException(string.Format("Property {0} doesn't have a name for synchronization", array.First<DataEntry.DataEntryColumn>((DataEntry.DataEntryColumn c) => string.IsNullOrEmpty(c.SynchronizableName)).PropertyInfo.Name));
			}
			return array;
		}

		public static bool InstanceExists<T>(int id)
		where T : DataEntry
		{
			return DataEntry.InstanceExists(typeof(T), id);
		}

		public static bool InstanceExists(Type type, int id)
		{
			if (!type.IsSubclassOf(typeof(DataEntry)))
			{
				throw new ArgumentException(string.Format("Type {0} is not subclass of {1}", type.Name, typeof(DataEntry).Name));
			}
			return (!DataEntry._dataEntriesPool.ContainsKey(type) ? false : DataEntry._dataEntriesPool[type].ContainsKey(id));
		}

		private static string[] ParseStringsArray(JsonTextReader reader)
		{
			if (reader.TokenType != JsonToken.StartArray)
			{
				throw new JsonException(string.Format("Token {0} expected", JsonToken.StartArray));
			}
			List<string> strs = new List<string>();
			if (!reader.Read())
			{
				throw new JsonException("JSON is finished unexpectedly");
			}
			while (reader.TokenType != JsonToken.EndArray)
			{
				string value = reader.Value as string;
				if (reader.TokenType != JsonToken.String || value == null)
				{
					throw new JsonException(string.Format("Unexpected token: {0}, value: {1}", reader.TokenType, reader.Value));
				}
				strs.Add(value);
				if (reader.Read())
				{
					continue;
				}
				throw new JsonException("JSON is finished unexpectedly");
			}
			return strs.ToArray();
		}

		public static JSONObject SerializeToColumnsAndDataJson<T>(T entry)
		where T : DataEntry
		{
			return DataEntry.SerializeToColumnsAndDataJson<T>(new T[] { entry });
		}

		public static JSONObject SerializeToColumnsAndDataJson<T>(T[] entries)
		where T : DataEntry
		{
			DataEntry.DataEntryColumn[] array = (
				from column in DataEntry.GetSynchronizableColumns<T>()
				where column.SynchronizableAttribute.Push
				select column).ToArray<DataEntry.DataEntryColumn>();
			JSONObject jSONObject = new JSONObject();
			DataEntry.DataEntryColumn[] dataEntryColumnArray = array;
			for (int i = 0; i < (int)dataEntryColumnArray.Length; i++)
			{
				jSONObject.Add(dataEntryColumnArray[i].SynchronizableName);
			}
			JSONObject jSONObject1 = JSONObject.arr;
			T[] tArray = entries;
			for (int j = 0; j < (int)tArray.Length; j++)
			{
				T t = tArray[j];
				JSONObject jSONObject2 = new JSONObject();
				DataEntry.DataEntryColumn[] dataEntryColumnArray1 = array;
				for (int k = 0; k < (int)dataEntryColumnArray1.Length; k++)
				{
					dataEntryColumnArray1[k].AddToJson(jSONObject2, t);
				}
				jSONObject1.Add(jSONObject2);
			}
			JSONObject jSONObject3 = new JSONObject();
			jSONObject3.AddField("COLUMNS", jSONObject);
			jSONObject3.AddField("DATA", jSONObject1);
			return jSONObject3;
		}

		public static JSONObject SerializeToJson<T>(T entry)
		where T : DataEntry
		{
			DataEntry.DataEntryColumn[] array = (
				from column in DataEntry.GetSynchronizableColumns<T>()
				where column.SynchronizableAttribute.Push
				select column).ToArray<DataEntry.DataEntryColumn>();
			JSONObject jSONObject = new JSONObject();
			DataEntry.DataEntryColumn[] dataEntryColumnArray = array;
			for (int i = 0; i < (int)dataEntryColumnArray.Length; i++)
			{
				dataEntryColumnArray[i].AddFieldToJson(jSONObject, entry);
			}
			return jSONObject;
		}

		internal virtual void WillBeSavedToDatabase()
		{
		}

		private class DataEntryColumn
		{
			public DataColumnAttribute DataColumnAttribute
			{
				get;
				private set;
			}

			public PropertyInfo PropertyInfo
			{
				get;
				private set;
			}

			public SynchronizableAttribute SynchronizableAttribute
			{
				get;
				private set;
			}

			public string SynchronizableName
			{
				get;
				private set;
			}

			private DataEntryColumn(DataColumnAttribute dataColumnAttribute, SynchronizableAttribute synchronizableAttribute, PropertyInfo propertyInfo)
			{
				string name;
				this.DataColumnAttribute = dataColumnAttribute;
				this.SynchronizableAttribute = synchronizableAttribute;
				this.PropertyInfo = propertyInfo;
				if (this.SynchronizableAttribute == null)
				{
					this.SynchronizableName = null;
				}
				else
				{
					this.SynchronizableName = this.SynchronizableAttribute.Name;
					if (string.IsNullOrEmpty(this.SynchronizableName))
					{
						if (this.DataColumnAttribute == null)
						{
							name = null;
						}
						else
						{
							name = this.DataColumnAttribute.Name;
						}
						this.SynchronizableName = name;
					}
				}
			}

			private void AddArrayToJson(JSONObject json, DataEntry entry)
			{
				json.Add(this.GetJsonOfArray(entry));
			}

			private void AddArrayToJsonAsField(JSONObject json, DataEntry entry)
			{
				json.AddField(this.SynchronizableName, this.GetJsonOfArray(entry));
			}

			public void AddFieldToJson(JSONObject json, DataEntry entry)
			{
				if (!this.PropertyInfo.PropertyType.IsArray)
				{
					json.AddField(this.SynchronizableName, this.GetValue(entry));
				}
				else
				{
					this.AddArrayToJsonAsField(json, entry);
				}
			}

			public void AddToJson(JSONObject json, DataEntry entry)
			{
				if (!this.PropertyInfo.PropertyType.IsArray)
				{
					json.Add(this.GetValue(entry));
				}
				else
				{
					this.AddArrayToJson(json, entry);
				}
			}

			public static DataEntry.DataEntryColumn Create(PropertyInfo propertyInfo)
			{
				DataColumnAttribute customAttribute;
				SynchronizableAttribute synchronizableAttribute;
				if (!propertyInfo.HasCustomAttribute<DataColumnAttribute>(true))
				{
					customAttribute = null;
				}
				else
				{
					customAttribute = propertyInfo.GetCustomAttribute<DataColumnAttribute>(true);
				}
				DataColumnAttribute dataColumnAttribute = customAttribute;
				if (!propertyInfo.HasCustomAttribute<SynchronizableAttribute>(true))
				{
					synchronizableAttribute = null;
				}
				else
				{
					synchronizableAttribute = propertyInfo.GetCustomAttribute<SynchronizableAttribute>(true);
				}
				SynchronizableAttribute synchronizableAttribute1 = synchronizableAttribute;
				if (dataColumnAttribute == null && synchronizableAttribute1 == null)
				{
					return null;
				}
				return new DataEntry.DataEntryColumn(dataColumnAttribute, synchronizableAttribute1, propertyInfo);
			}

			private JSONObject GetJsonOfArray(DataEntry entry)
			{
				Type elementType = this.PropertyInfo.PropertyType.GetElementType();
				JSONObject jSONObject = JSONObject.arr;
				Array value = (Array)this.PropertyInfo.GetValue(entry, null);
				for (int i = 0; i < value.Length; i++)
				{
					object obj = value.GetValue(i);
					jSONObject.Add(ReflectionUtilities.GetValue(elementType, obj));
				}
				return jSONObject;
			}

			public string GetValue(DataEntry entry)
			{
				return entry.GetValue<DataEntry>(this.PropertyInfo);
			}

			public void SetValue(DataEntry entry, string value)
			{
				entry.SetValue<DataEntry>(this.PropertyInfo, value);
			}
		}
	}
}