using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Services.Interface
{
    public interface IAdminService
    {
        public Task<JsonResponse> GetAllCandidate(int UserId);
        public Task<JsonResponse> GetAllRecuiter(int UserId);

    }
}
