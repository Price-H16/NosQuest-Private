// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using ChickenAPI.DAL;
using Mapster;

namespace WingsEmu.DAL.EF.DAO.Utils
{
    public class MapsterMapper<TEntity, TDto> : IMapper<TEntity, TDto> where TDto : new() where TEntity : new()
    {
        public bool MapToDto(TEntity input, out TDto output)
        {
            output = new TDto();
            input.Adapt(output);
            return true;
        }

        public bool MapToDtos(IReadOnlyCollection<TEntity> input, out IEnumerable<TDto> output)
        {
            output = new TDto[input.Count];
            input.Adapt(output);
            return true;
        }

        public bool MapToEntity(TDto input, out TEntity output)
        {
            output = new TEntity();
            input.Adapt(output);
            return true;
        }

        public bool MapToEntities(IEnumerable<TDto> input, out IEnumerable<TEntity> output)
        {
            output = new List<TEntity>();
            input.Adapt(output);
            return true;
        }
    }
}