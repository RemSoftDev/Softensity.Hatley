using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CookComputing.XmlRpc;
using InfusionSoft.Tables;

namespace InfusionSoft.Custom
{
	public static class DataServiceExtensionsCustom
	{
		public static IEnumerable<T> Query<T>(this IDataService service, string apiKey) where T : ITable, new()
		{
			return Query<T>(service, apiKey, q => q.Empty(), p => p.IncludeAll());
		}

		public static IEnumerable<T> Query<T>(this IDataService service, string apiKey, string orderBy) where T : ITable, new()
		{
			return Query<T>(service, apiKey, q => q.Empty(), p => p.IncludeAll(), orderBy);
		}

		public static IEnumerable<T> Query<T>(this IDataService service, string apiKey, string orderBy, bool asc) where T : ITable, new()
		{
			return Query<T>(service, apiKey, q => q.Empty(), p => p.IncludeAll(), orderBy, asc);
		}

		public static IEnumerable<T> Query<T>(this IDataService service, string apiKey, Action<IQueryBuilder<T>> queryBuilder)
			where T : ITable, new()
		{
			return Query(service, apiKey, queryBuilder, p => p.IncludeAll());
		}

		public static IEnumerable<T> Query<T>(this IDataService service, string apiKey, Action<IQueryBuilder<T>> queryBuilder, string orderBy)
			where T : ITable, new()
		{
			return Query(service, apiKey, queryBuilder, p => p.IncludeAll(), orderBy);
		}

		public static IEnumerable<T> Query<T>(this IDataService service, string apiKey, Action<IQueryBuilder<T>> queryBuilder, string orderBy, bool asc)
			where T : ITable, new()
		{
			return Query(service, apiKey, queryBuilder, p => p.IncludeAll(), orderBy, asc);
		}

		public static IEnumerable<T> Query<T>(this IDataService service, string apiKey, Action<IProjection<T>> projection)
			where T : ITable, new()
		{
			return Query(service, apiKey, q => q.Empty(), projection);
		}

		public static IEnumerable<T> Query<T>(this IDataService service, string apiKey, Action<IProjection<T>> projection, string orderBy)
			where T : ITable, new()
		{
			return Query(service, apiKey, q => q.Empty(), projection, orderBy);
		}

		public static IEnumerable<T> Query<T>(this IDataService service, string apiKey, Action<IProjection<T>> projection, string orderBy, bool asc)
			where T : ITable, new()
		{
			return Query(service, apiKey, q => q.Empty(), projection, orderBy, asc);
		}

		public static IEnumerable<T> Query<T>(this IDataService service, string apiKey, Action<IQueryBuilder<T>> queryBuilder,
											  Action<IProjection<T>> projection)
			where T : ITable, new()
		{
			// ReSharper disable RedundantTypeArgumentsOfMethod
			return GetAllPages<T, Action<IQueryBuilder<T>>, Action<IProjection<T>>, string, bool>(service.Query, queryBuilder,
																					projection, String.Empty, false, apiKey);
			// ReSharper restore RedundantTypeArgumentsOfMethod
		}

		public static IEnumerable<T> Query<T>(this IDataService service, string apiKey, Action<IQueryBuilder<T>> queryBuilder,
											  Action<IProjection<T>> projection, string orderBy)
			where T : ITable, new()
		{
			// ReSharper disable RedundantTypeArgumentsOfMethod
			return GetAllPages<T, Action<IQueryBuilder<T>>, Action<IProjection<T>>, string, bool>(service.Query, queryBuilder,
																					projection, orderBy, false, apiKey);
			// ReSharper restore RedundantTypeArgumentsOfMethod
		}

		public static IEnumerable<T> Query<T>(this IDataService service, string apiKey, Action<IQueryBuilder<T>> queryBuilder,
											  Action<IProjection<T>> projection, string orderBy, bool asc)
			where T : ITable, new()
		{
			// ReSharper disable RedundantTypeArgumentsOfMethod
			return GetAllPages<T, Action<IQueryBuilder<T>>, Action<IProjection<T>>, string, bool>(service.Query, queryBuilder,
																					projection, orderBy, asc, apiKey);
			// ReSharper restore RedundantTypeArgumentsOfMethod
		}

		public static IEnumerable<T> Query<T>(this IDataService service, string apiKey, DataPage page) where T : ITable, new()
		{
			return Query<T>(service, apiKey, page, builder => builder.Empty(), projection => projection.IncludeAll());
		}

		public static IEnumerable<T> Query<T>(this IDataService service, string apiKey, DataPage page, string orderBy) where T : ITable, new()
		{
			return Query<T>(service, apiKey, page, builder => builder.Empty(), projection => projection.IncludeAll(), orderBy);
		}

		public static IEnumerable<T> Query<T>(this IDataService service, string apiKey, DataPage page, string orderBy, bool asc) where T : ITable, new()
		{
			return Query<T>(service, apiKey, page, builder => builder.Empty(), projection => projection.IncludeAll(), orderBy, asc);
		}

		public static IEnumerable<T> Query<T>(this IDataService service, string apiKey, DataPage page,
											  Action<IQueryBuilder<T>> queryBuilder) where T : ITable, new()
		{
			return Query(service, apiKey, page, queryBuilder, p => p.IncludeAll());
		}

		public static IEnumerable<T> Query<T>(this IDataService service, string apiKey, DataPage page,
											  Action<IQueryBuilder<T>> queryBuilder, string orderBy) where T : ITable, new()
		{
			return Query(service, apiKey, page, queryBuilder, p => p.IncludeAll(), orderBy);
		}

		public static IEnumerable<T> Query<T>(this IDataService service, string apiKey, DataPage page,
											  Action<IQueryBuilder<T>> queryBuilder, string orderBy, bool asc) where T : ITable, new()
		{
			return Query(service, apiKey, page, queryBuilder, p => p.IncludeAll(), orderBy, asc);
		}

		public static IEnumerable<T> Query<T>(this IDataService service, string apiKey, DataPage page,
											  Action<IProjection<T>> projection) where T : ITable, new()
		{
			return Query(service, apiKey, page, builder => builder.Empty(), projection);
		}

		public static IEnumerable<T> Query<T>(this IDataService service, string apiKey, DataPage page,
											 Action<IProjection<T>> projection, string orderBy) where T : ITable, new()
		{
			return Query(service, apiKey, page, builder => builder.Empty(), projection, orderBy);
		}

		public static IEnumerable<T> Query<T>(this IDataService service, string apiKey, DataPage page,
											 Action<IProjection<T>> projection, string orderBy, bool asc) where T : ITable, new()
		{
			return Query(service, apiKey, page, builder => builder.Empty(), projection, orderBy, asc);
		}

		public static IEnumerable<T> Query<T>(this IDataService service, string apiKey, DataPage page,
											  Action<IQueryBuilder<T>> queryBuilder,
											  Action<IProjection<T>> fieldSelection) where T : ITable, new()
		{
			return Query(service, apiKey, page, queryBuilder, fieldSelection, String.Empty, false);
		}

		public static IEnumerable<T> Query<T>(this IDataService service, string apiKey, DataPage page,
											 Action<IQueryBuilder<T>> queryBuilder,
											 Action<IProjection<T>> fieldSelection, string orderBy) where T : ITable, new()
		{
			return Query(service, apiKey, page, queryBuilder, fieldSelection, orderBy, false);
		}

		public static IEnumerable<T> Query<T>(this IDataService service, string apiKey, DataPage page,
											  Action<IQueryBuilder<T>> queryBuilder,
											  Action<IProjection<T>> fieldSelection, string orderBy, bool asc) where T : ITable, new()
		{
			var query = new QueryBuilder<T>();
			queryBuilder(query);

			var projection = new Projection<T>();
			fieldSelection(projection);

			XmlRpcStruct data = query.Dictionary.AsXmlRpcStruct();
			string[] selectedFields = projection.Build();

			var wrapper = new DataServiceWrapperCustom(apiKey);

			if (String.IsNullOrEmpty(orderBy))
			{
				return wrapper.Invoke<IEnumerable<object>, T[]>(d => d.Query(apiKey,
																			 typeof(T).Name, page.Size,
																			 page.Number, data, selectedFields));
			}
			else
			{
				return wrapper.Invoke<IEnumerable<object>, T[]>(d => d.Query(apiKey,
																			 typeof(T).Name, page.Size,
																			 page.Number, data, selectedFields, orderBy, asc));
			}
		}

		public static IEnumerable<T> GetAllPages<T, T1, T2>(Func<DataPage, T1, T2, IEnumerable<T>> func, T1 arg1,
															T2 arg2)
		{
			var list = new List<T>();
			T[] collection;

			DataPage page = DataPage.First;

			do
			{
				collection = func(page, arg1, arg2).ToArray();

				list.AddRange(collection);

				page = page.Next();

				//NOTE: Don't try to fetch more records if we know there aren't any more.
				if (collection.Length < DataPage.MaxSize)
					break;
			} while (collection.Length > 0);

			return list;
		}

		public static IEnumerable<T> GetAllPages<T, T1, T2, T3, T4>(Func<string, DataPage, T1, T2, string, bool, IEnumerable<T>> func, T1 arg1,
														   T2 arg2, string orderBy, bool asc, string apiKey)
		{
			var list = new List<T>();
			T[] collection;

			DataPage page = DataPage.First;

			do
			{
				collection = func(apiKey, page, arg1, arg2, orderBy, asc).ToArray();

				list.AddRange(collection);

				page = page.Next();

				//NOTE: Don't try to fetch more records if we know there aren't any more.
				if (collection.Length < DataPage.MaxSize)
					break;
			} while (collection.Length > 0);

			return list;
		}
	}
}
