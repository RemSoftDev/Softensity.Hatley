using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfusionSoft.Definition;

namespace InfusionSoft.Custom
{
	public class DataServiceWrapperCustom : ServiceBaseCustom<IDataServiceDefinition>, IDataService
	{

		public DataServiceWrapperCustom(string apiKey) :
            base("https://api.infusionsoft.com/crm/xmlrpc/v1?access_token=" + apiKey)
		{
		}

		public virtual int Add(string table, System.Collections.IDictionary values)
		{
			return Invoke(d => d.Add(ApiKey, table, values));
		}

		public virtual object Load(string table, int recordId, string[] wantedFields)
		{
			return Invoke(d => d.Load(ApiKey, table, recordId, wantedFields));
		}

		public virtual int Update(string table, int id, object values)
		{
			return Invoke(d => d.Update(ApiKey, table, id, values));
		}

		public virtual bool Delete(string table, int id)
		{
			return Invoke(d => d.Delete(ApiKey, table, id));
		}

		public virtual System.Collections.Generic.IEnumerable<object> FindByField(string table, int limit, int page,
			string fieldName, object fieldValue, string[] returnFields)
		{
			return Invoke(d => d.FindByField(ApiKey, table, limit, page, fieldName, fieldValue, returnFields));
		}

		public virtual System.Collections.Generic.IEnumerable<object> Query(string table, int limit, int page,
			System.Collections.IDictionary queryData, string[] selectedFields)
		{
			return Invoke(d => d.Query(ApiKey, table, limit, page, queryData, selectedFields));
		}

		public virtual System.Collections.Generic.IEnumerable<object> Query(string table, int limit, int page,
			System.Collections.IDictionary queryData, string[] selectedFields, string orderBy, bool asc)
		{
			return Invoke(d => d.Query(ApiKey, table, limit, page, queryData, selectedFields, orderBy, asc));
		}

		public virtual int AddCustomField(string customFieldType, string displayName, string dataType, int headerId)
		{
			return Invoke(d => d.AddCustomField(ApiKey, customFieldType, displayName, dataType, headerId));
		}

		public virtual int AuthenticateUser(string username, string passwordHash)
		{
			return Invoke(d => d.AuthenticateUser(ApiKey, username, passwordHash));
		}

		public virtual string GetAppSetting(string module, string setting)
		{
			return Invoke(d => d.GetAppSetting(ApiKey, module, setting));
		}

		public virtual string GetTemporaryKey(string username, string passwordHash)
		{
			return Invoke(d => d.GetTemporaryKey(ApiKey, username, passwordHash));
		}

		public virtual bool UpdateCustomField(int customFieldId, System.Collections.IDictionary values)
		{
			return Invoke(d => d.UpdateCustomField(ApiKey, customFieldId, values));
		}
	}
}
