// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;

namespace ChickenAPI.DAL
{
    /// <summary>
    ///     IMapper facilitate mapping between an entity and a dto
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TDto"></typeparam>
    public interface IMapper<TEntity, TDto>
    {
        /// <summary>
        ///     Maps the entity to its
        ///     <typeparam name="TDto"></typeparam>
        ///     from the given
        ///     <typeparam name="TEntity"></typeparam>
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns>false if the mapping failed</returns>
        bool MapToDto(TEntity input, out TDto output);

        /// <summary>
        ///     Maps all the given
        ///     <typeparam name="TEntity"></typeparam>
        ///     to their corresponding
        ///     <typeparam name="TDto"></typeparam>
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns>false if the mapping failed</returns>
        bool MapToDtos(IReadOnlyCollection<TEntity> input, out IEnumerable<TDto> output);

        /// <summary>
        ///     Maps the entity to its
        ///     <typeparam name="TEntity"></typeparam>
        ///     from the given
        ///     <typeparam name="TDto"></typeparam>
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns>false if the mapping failed</returns>
        bool MapToEntity(TDto input, out TEntity output);

        /// <summary>
        ///     Maps all the given
        ///     <typeparam name="TDto"></typeparam>
        ///     to their corresponding
        ///     <typeparam name="TEntity"></typeparam>
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns>false if the mapping failed</returns>
        bool MapToEntities(IEnumerable<TDto> input, out IEnumerable<TEntity> output);
    }
}