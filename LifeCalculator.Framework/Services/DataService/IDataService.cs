using System.Collections.Generic;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.Services.DataService
{
    public interface IDataService<T>
    {
        Task<IEnumerable<T>> LoadAll();
        Task<T> Load(int id);
        Task Save(T entity);
        Task Update(int id, T entity);
        Task Delete(int id);

    }
}
