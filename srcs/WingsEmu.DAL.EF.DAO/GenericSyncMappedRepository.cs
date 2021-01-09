// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using ChickenAPI.DAL;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using WingsEmu.DAL.EF.DAO.Utils;

namespace WingsEmu.DAL.EF.DAO
{
    public sealed class GenericSyncMappedRepository<TEntity, TDto> : IGenericSyncMappedRepository<TDto>
    where TDto : class, IMappedDto, new()
    where TEntity : class, IMappedEntity, new()
    {
        private readonly IContextFactory _contextFactory;
        private readonly IMapper<TEntity, TDto> _mapper;

        public GenericSyncMappedRepository(IContextFactory contextFactory, IMapper<TEntity, TDto> mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public IEnumerable<TDto> GetAll()
        {
            using (DbContext context = _contextFactory.NewContext())
            {
                List<TEntity> tmp = context.Set<TEntity>().ToList();
                if (!_mapper.MapToDtos(tmp.AsReadOnly(), out IEnumerable<TDto> dtos))
                {
                    return null;
                }

                return dtos;
            }
        }

        public IEnumerable<TDto> GetAllMatching(Func<TDto, bool> predicate)
        {
            try
            {
                using (DbContext context = _contextFactory.NewContext())
                {
                    Expression<Func<TEntity, bool>> compiled = predicate.Convert<TDto, TEntity>();
                    List<TEntity> tmp = context.Set<TEntity>().Where(compiled).ToList();
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

        public TDto GetFirstMatchingOrDefault(Func<TDto, bool> predicate)
        {
            try
            {
                using (DbContext context = _contextFactory.NewContext())
                {
                    Expression<Func<TEntity, bool>> compiled = predicate.Convert<TDto, TEntity>();
                    TEntity tmp = context.Set<TEntity>().FirstOrDefault(compiled);
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

        public TDto GetById(int id)
        {
            try
            {
                using (DbContext context = _contextFactory.NewContext())
                {
                    TEntity tmp = context.Set<TEntity>().SingleOrDefault(s => s.Id == id);
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

        public IEnumerable<TDto> GetByIds(IEnumerable<int> ids)
        {
            try
            {
                using (DbContext context = _contextFactory.NewContext())
                {
                    List<TEntity> tmp = context.Set<TEntity>().Where(s => ids.Any(id => s.Id == id)).ToList();
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

        public TDto Save(TDto obj)
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

                        entity = context.Set<TEntity>().Add(entity).Entity;
                    }
                    else
                    {
                        context.Entry(entity).CurrentValues.SetValues(obj);
                    }

                    if (!_mapper.MapToDto(entity, out obj))
                    {
                        return null;
                    }

                    context.SaveChanges();
                    return obj;
                }
            }
            catch (Exception e)
            {
                // todo log
                return null;
            }
        }

        public List<TDto> Save(IEnumerable<TDto> objs)
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
                            context.BulkInsertOrUpdate(entitiesList, new BulkConfig
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

                    context.SaveChanges();
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

        public void DeleteById(int id)
        {
            try
            {
                using (DbContext context = _contextFactory.NewContext())
                {
                    var model = new TEntity { Id = id };
                    context.Set<TEntity>().Attach(model);
                    context.Set<TEntity>().Remove(model);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                // log the exception
            }
        }

        public void DeleteByIds(IEnumerable<int> ids)
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
                            context.BulkDelete(toDelete);
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