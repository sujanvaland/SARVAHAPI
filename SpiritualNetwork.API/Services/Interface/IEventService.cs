using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Services.Interface
{
    public interface IEventService
    {
        Task<Event> SaveEvent(Event poll);
        Task<JsonResponse> GetEventList(int PageNo, int Size);

        Task<JsonResponse> GetEventTypeList();
    }
}
