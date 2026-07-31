using System.Threading.Tasks;

namespace SenangMemberApp.Shared.Services.IService
{
    public interface IUrlLauncher
    {
        Task<bool> OpenUrlAsync(string url);
    }
}
