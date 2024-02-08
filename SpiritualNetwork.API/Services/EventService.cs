using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Services
{
    public class EventService : IEventService
    {
        private readonly IRepository<Event> _eventRepository;
        private readonly IRepository<EventType> _eventtypeRepository;
        public EventService(IRepository<Event> eventRepository, IRepository<EventType> eventtypeRepository)
        {
            _eventRepository = eventRepository; _eventtypeRepository = eventtypeRepository;
        }

        public async Task<Event> SaveEvent(Event events)
        {
            if (events.Id == 0)
            {
                await _eventRepository.InsertAsync(events);
            }
            else
            {
                await _eventRepository.UpdateAsync(events);
            }

            return events;
        }
        public async Task<JsonResponse> GetEventList(int PageNo, int Size)
        {

            var eventsquery = (from ev in _eventRepository.Table
                          select new EventModel
                          {
                              Id = ev.Id,
                              EventTitle = ev.EventTitle,
                              EventFormat = ev.EventFormat,
                              EventTypeId = ev.EventTypeId,
                              StartDate = ev.StartDate,
                              EndDate = ev.EndDate,
                              StartTime = ev.StartTime,
                              EndTime = ev.EndTime,
                              Description = ev.Description,
                              EventLink = ev.EventLink,
                              Eventaddress = ev.Eventaddress,
                              EventCoverImage = ev.EventCoverImage
                          });

            var eventlist = await eventsquery.Take(Size).Skip((PageNo - 1) * Size).ToListAsync();

            return new JsonResponse(200, true, "Success", eventlist);
        }

        public async Task<JsonResponse> GetEventTypeList()
        {
            var eventtypelist = await  _eventtypeRepository.Table.ToListAsync();

            return new JsonResponse(200, true, "Success", eventtypelist);
        }
    }
}
