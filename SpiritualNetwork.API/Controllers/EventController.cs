using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EventController : ApiBaseController
    {
        private readonly IEventService _eventService;
        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }


        [HttpPost(Name = "SaveEvent")]
        public async Task<JsonResponse> SaveEvent(Event eventdata)
        {
            try
            {
                var response = await _eventService.SaveEvent(eventdata);
                return new JsonResponse(200, true, "Success", response);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }
        [HttpPost(Name = "GetEventList")]
        public async Task<JsonResponse> GetEventList()
        {
            try
            {
                var response = await _eventService.GetEventList(1, 100);
                return new JsonResponse(200, true, "Success", response);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }
        [HttpPost(Name = "GetEventTypeList")]
        public async Task<JsonResponse> GetEventTypeList()
        {
            try
            {
                var response = await _eventService.GetEventTypeList();
                return new JsonResponse(200, true, "Success", response);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }
    }
}
