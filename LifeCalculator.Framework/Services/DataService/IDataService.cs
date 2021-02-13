using System.Collections.Generic;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.Services.DataService
{
    public interface IDataService<T>
    {
        Task<IEnumerable<T>> LoadAll();
        Task<T> Load(int id);
        Task Insert(T entity);
        Task Save(int id, T entity);
        Task Delete(int id);

    }
}
