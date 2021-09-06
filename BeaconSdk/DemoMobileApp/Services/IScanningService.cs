using System;
using System.Threading.Tasks;

namespace DemoMobileApp.Services
{
    public interface IScanningService
    {
        Task<string> ScanAsync();
    }
}
