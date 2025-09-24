namespace Application.Interfaces.Repositories.Base
{
    public interface IBaseRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetByIDAsync();
        Task<IEnumerable<T>> AddAsync(T entity);
        Task<IEnumerable<T>> UpdateAsync(T entity);
        Task<IEnumerable<T>> DeleteAsync(T entity);




    }
}
