﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SearchController : ApiBaseController
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpPost(Name = "SearchUser")]
        public async Task<JsonResponse> SearchUser(SearchReqByPage req)
        {
            try
            {
                var response = await _searchService.SearchUser(req.Name, req.PageNo, req.Records);
                return response;
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "MentionSearchUser")]
        public async Task<JsonResponse> MentionSearchUser(SearchReqByPage req)
        {
            try
            {
                var response = await _searchService.MentionSearchUser(req.Name, req.PageNo, req.Records);
                return response;
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost(Name = "SearchedUserData")]
        public async Task<JsonResponse> SearchedUserData(SearchReq searchreq)
        {
            try
            {
                var response = await _searchService.SearchUserProfile(searchreq.Name);
                return response;
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

    }
}
