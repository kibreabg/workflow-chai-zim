using System.Collections;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace Chai.WorkflowManagment.CoreDomain.Infrastructure
{
	public class BaseDbContext : DbContext
	{
		public BaseDbContext(string name)
			: base(name)
		{

		}

		public IQueryable<T> ReadOnly<T>() where T : class
		{
			ObjectSet<T> result = ObjContext().CreateObjectSet<T>();
			result.MergeOption = MergeOption.NoTracking;
			return result;
		}

		public IQueryable<T> Trackable<T>() where T : class
		{
			return ObjContext().CreateObjectSet<T>();
		}

		public void Refresh(IEnumerable collection)
		{
			ObjContext().Refresh(RefreshMode.StoreWins, collection);
		}

		public void Refresh(object item)
		{
			ObjContext().Refresh(RefreshMode.StoreWins, item);
		}

		public void Detach(object item)
		{
			ObjContext().Detach(item);
		}

		public void LoadProperty(object item, string propertyName)
		{
			ObjContext().LoadProperty(item, propertyName);
		}

		public void Close()
		{
			ObjContext().Connection.Close();
		}

		public ObjectContext ObjContext()
		{
			return ((IObjectContextAdapter)this).ObjectContext;
		}

		public void AcceptAllChanges()
		{
			ObjContext().AcceptAllChanges();
		}

		public int ExecuteFunction(string functionName, params ObjectParameter[] parameters)
		{
			return ObjContext().ExecuteFunction(functionName, parameters);
		}
	}
}
