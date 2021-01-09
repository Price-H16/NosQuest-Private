// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChickenAPI.DAL;

namespace WingsEmu.DAL.EF.DAO
{
    /// <summary>
    ///     GenericAsyncMappedRepository is an asynchronous & synchronous Data Access Object
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TDto"></typeparam>
    public sealed class GenericMappedRepository<TEntity, TDto> : IGenericAsyncMappedRepository<TDto>, IGenericSyncMappedRepository<TDto>
    where TDto : class, IMappedDto
    where TEntity : class, IMappedEntity, new()
    {
        private readonly IGenericAsyncMappedRepository<TDto> _genericAsync;
        private readonly IGenericSyncMappedRepository<TDto> _genericSync;

        public GenericMappedRepository(IGenericAsyncMappedRepository<TDto> genericAsync, IGenericSyncMappedRepository<TDto> genericSync)
        {
            _genericAsync = genericAsync;
            _genericSync = genericSync;
        }

        public Task<IEnumerable<TDto>> GetAllAsync() => _genericAsync.GetAllAsync();

        public Task<IEnumerable<TDto>> GetAllMatchingAsync(Func<TDto, bool> predicate) => _genericAsync.GetAllMatchingAsync(predicate);

        public Task<TDto> GetFirstMatchingOrDefaultAsync(Func<TDto, bool> predicate) => _genericAsync.GetFirstMatchingOrDefaultAsync(predicate);

        public Task<TDto> GetByIdAsync(int id) => _genericAsync.GetByIdAsync(id);

        public Task<IEnumerable<TDto>> GetByIdsAsync(IEnumerable<int> ids) => _genericAsync.GetByIdsAsync(ids);

        public Task<TDto> SaveAsync(TDto obj) => _genericAsync.SaveAsync(obj);

        public Task<List<TDto>> SaveAsync(IEnumerable<TDto> objs) => _genericAsync.SaveAsync(objs);

        public Task DeleteByIdAsync(int id) => _genericAsync.DeleteByIdAsync(id);

        public Task DeleteByIdsAsync(IEnumerable<int> ids) => _genericAsync.DeleteByIdsAsync(ids);


        public IEnumerable<TDto> GetAll() => _genericSync.GetAll();

        IEnumerable<TDto> IGenericSyncRepository<TDto, int>.GetAllMatching(Func<TDto, bool> predicate) => _genericSync.GetAllMatching(predicate);

        TDto IGenericSyncRepository<TDto, int>.GetFirstMatchingOrDefault(Func<TDto, bool> predicate) => _genericSync.GetFirstMatchingOrDefault(predicate);

        public TDto GetById(int id) => _genericSync.GetById(id);

        public IEnumerable<TDto> GetByIds(IEnumerable<int> ids) => _genericSync.GetByIds(ids);

        public TDto Save(TDto obj) => _genericSync.Save(obj);
        List<TDto> IGenericSyncRepository<TDto, int>.Save(IEnumerable<TDto> objs) => _genericSync.Save(objs);

        public void DeleteById(int id)
        {
            _genericSync.DeleteById(id);
        }

        public void DeleteByIds(IEnumerable<int> ids)
        {
            _genericSync.DeleteByIds(ids);
        }

        public void Save(IEnumerable<TDto> objs)
        {
            _genericSync.Save(objs);
        }
    }
}