// WingsEmu
// 
// Developed by NosWings Team

namespace ChickenAPI.DAL
{
    public interface IGenericSyncMappedRepository<T> : IGenericSyncRepository<T, int> where T : class, IMappedDto
    {
    }
}