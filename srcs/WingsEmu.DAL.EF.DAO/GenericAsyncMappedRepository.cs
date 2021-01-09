// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ChickenAPI.DAL;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using WingsEmu.DAL.EF.DAO.Utils;

namespace WingsEmu.DAL.EF.DAO
{
    /// <summary>
    ///     GenericAsyncMappedRepository is an asynchronous Data Access Object
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TDto"></typeparam>
    public sealed class GenericAsyncMappedRepository<TEntity, TDto> : IGenericAsyncMappedRepository<TDto>
    where TDto : class, IMappedDto, new()
    where TEntity : class, IMappedEntity, new()
    {
        private readonly IContextFactory _contextFactory;
        private readonly IMapper<TEntity, TDto> _mapper;

        public GenericAsyncMappedRepository(IContextFactory contextFactory, IMapper<TEntity, TDto> mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TDto>> GetAllAsync()
        {
            using (DbContext context = _contextFactory.NewContext())
            {
                List<TEntity> tmp = await context.Set<TEntity>().ToListAsync();
                if (!_mapper.MapToDtos(tmp.AsReadOnly(), out IEnumerable<TDto> dtos))
                {
                    return null;
                }

                return dtos;
            }
        }

        public async Task<IEnumerable<TDto>> GetAllMatchingAsync(Func<TDto, bool> predicate)
        {
            try
            {
                using (DbContext context = _contextFactory.NewContext())
                {
                    Expression<Func<TEntity, bool>> compiled = predicate.Convert<TDto, TEntity>();
                    List<TEntity> tmp = await context.Set<TEntity>().Where(compiled).ToListAsync();
                    if (!_mapper.MapToDtos(tmp.AsReadOnly(), out IEnumerable<TDto> dtos))
                    {
                        return null;
                    }

                    return dtos;
                }
            }
            catch (Exception e)
            {
                // log the exception
                return null;
            }
        }

        public async Task<TDto> GetFirstMatchingOrDefaultAsync(Func<TDto, bool> predicate)
        {
            try
            {
                using (DbContext context = _contextFactory.NewContext())
                {
                    Expression<Func<TEntity, bool>> compiled = predicate.Convert<TDto, TEntity>();
                    TEntity tmp = await context.Set<TEntity>().FirstOrDefaultAsync(compiled);
                    if (!_mapper.MapToDto(tmp, out TDto dto))
                    {
                        return null;
                    }

                    return dto;
                }
            }
            catch (Exception e)
            {
                // log the exception
                return null;
            }
        }

        public async Task<TDto> GetByIdAsync(int id)
        {
            try
            {
                using (DbContext context = _contextFactory.NewContext())
                {
                    TEntity tmp = await context.Set<TEntity>().FirstOrDefaultAsync(s => s.Id == id);
                    if (!_mapper.MapToDto(tmp, out TDto dto))
                    {
                        return null;
                    }

                    return dto;
                }
            }
            catch (Exception e)
            {
                // log the exception
                return null;
            }
        }

        public async Task<IEnumerable<TDto>> GetByIdsAsync(IEnumerable<int> ids)
        {
            try
            {
                using (DbContext context = _contextFactory.NewContext())
                {
                    List<TEntity> tmp = await context.Set<TEntity>().Where(s => ids.Any(id => s.Id == id)).ToListAsync();
                    if (!_mapper.MapToDtos(tmp, out IEnumerable<TDto> dto))
                    {
                        return null;
                    }

                    return dto;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<TDto> SaveAsync(TDto obj)
        {
            try
            {
                using (DbContext context = _contextFactory.NewContext())
                {
                    TEntity entity = context.Set<TEntity>().Find(obj.Id);

                    if (entity == null)
                    {
                        if (!_mapper.MapToEntity(obj, out entity))
                        {
                            // the mapping didn't succeed, we should log it
                            return null;
                        }

                        entity = (await context.Set<TEntity>().AddAsync(entity)).Entity;
                    }
                    else
                    {
                        context.Entry(entity).CurrentValues.SetValues(obj);
                    }

                    if (!_mapper.MapToDto(entity, out obj))
                    {
                        return null;
                    }

                    await context.SaveChangesAsync();
                    return obj;
                }
            }
            catch (Exception e)
            {
                // todo log
                return null;
            }
        }

        public async Task<List<TDto>> SaveAsync(IEnumerable<TDto> objs)
        {
            try
            {
                using (DbContext context = _contextFactory.NewContext())
                {
                    if (!_mapper.MapToEntities(objs, out IEnumerable<TEntity> entities))
                    {
                        return null;
                    }

                    List<TEntity> entitiesList = entities.ToList();
                    using (IDbContextTransaction transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            await context.BulkInsertOrUpdateAsync(entitiesList, new BulkConfig
                            {
                                PreserveInsertOrder = true,
                                SqlBulkCopyOptions = SqlBulkCopyOptions.KeepIdentity
                            });
                            transaction.Commit();
                        }
                        catch (Exception e)
                        {
                            // log exception
                            transaction.Rollback();
                        }
                    }

                    await context.SaveChangesAsync();
                    if (!_mapper.MapToDtos(entitiesList, out objs))
                    {
                        return null;
                    }

                    return objs.ToList();
                }
            }
            catch (Exception e)
            {
                return null;
                // log exception
            }
        }

        public async Task DeleteByIdAsync(int id)
        {
            try
            {
                using (DbContext context = _contextFactory.NewContext())
                {
                    var model = new TEntity { Id = id };
                    context.Set<TEntity>().Attach(model);
                    context.Set<TEntity>().Remove(model);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                // log the exception
            }
        }

        public async Task DeleteByIdsAsync(IEnumerable<int> ids)
        {
            try
            {
                using (DbContext context = _contextFactory.NewContext())
                {
                    using (IDbContextTransaction transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            List<TEntity> toDelete = ids.Select(s => new TEntity { Id = s }).ToList();
                            await context.BulkDeleteAsync(toDelete);
                            transaction.Commit();
                        }
                        catch (Exception e)
                        {
                            transaction.Rollback();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // log exception
            }
        }
    }
}