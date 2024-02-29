﻿using SpiritualNetwork.API.Model;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Services.Interface
{
    public interface IProfileService
    {
        Task<ProfileModel> GetUserProfileByUsername(string username);
        Task<ProfileModel> GetUserInfoBox(string username, int UserId);
        Task<ProfileModel> GetUserInfoBoxByUserId(int UserId, int LoginUserId);
        public Task<JsonResponse> UpdateProfile(ProfileReqest profileReq, int UserId);
        public Task<JsonResponse> GetBooksSuggestion(int UserId);
        public Task<JsonResponse> GetMoviesSuggestion(int UserId);
        public Task<JsonResponse> GetGurusSuggestion(int UserId);
        public Task<JsonResponse> GetPracticeSuggestion(int UserId);
        public Task<JsonResponse> GetExperienceSuggestion(int UserId);
        public Task<JsonResponse> AddSuggestion(BookMarkRes res, int UserId);
        public Task<JsonResponse> SearchSuggestion(SearchProfileSuggestion req);
        public Task<JsonResponse> UpdateIsReadSuggestion(int Id);
        public Task<JsonResponse> DeleteProfileSuggestion(int Id);
        Task<ProfileModel> GetUserProfileById(int Id);
        public ProfileModel GetUserProfile(User user);
    }
}
