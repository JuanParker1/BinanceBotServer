using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceBotApp.Services
{
    public interface ICrudService<TDto>
        where TDto : Data.IId
    {
        IEnumerable<string> Includes { get; }
        Task<TDto> GetAsync(int id, CancellationToken token);
        Task<IEnumerable<TDto>> GetAllAsync(CancellationToken token);
        Task<int> InsertAsync(TDto newItem, CancellationToken token);
        Task<int> InsertRangeAsync(IEnumerable<TDto> newItems, CancellationToken token);
        Task<int> UpdateAsync(int id, TDto item, CancellationToken token);
        Task<int> DeleteAsync(int id, CancellationToken token);
        Task<int> DeleteRangeAsync(IEnumerable<int> ids, CancellationToken token);
    }
}