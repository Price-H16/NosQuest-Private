// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Master.Managers
{
    public class MaintenanceManager
    {
        private bool _isInMaintenance;

        public void SetMaintenanceMode(bool value)
        {
            _isInMaintenance = value;
        }

        public bool GetMaintenanceMode() => _isInMaintenance;
    }
}