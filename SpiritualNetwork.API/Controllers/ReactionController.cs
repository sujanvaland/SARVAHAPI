﻿using Microsoft.AspNetCore.Mvc;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReactionController : ApiBaseController
    {
        private readonly IReactionService _reactionService;
        public ReactionController(IReactionService reactionService)
        {
            _reactionService = reactionService;
        }

        [HttpPost(Name = "GetAllReactions")]
        public async Task<JsonResponse> GetAllReactions(GetAllReactionReq reactionReq)
        {
            try
            {
                return await _reactionService.GetAllReaction(reactionReq.PostId);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpGet(Name = "GetBookmarks")]
        public async Task<JsonResponse> GetBookmarks()
        {
            try
            {
                return await _reactionService.GetAllBookmarksByUserId(user_unique_id);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "ToggleBookmark")]
        public async Task<JsonResponse> ToggleBookmark(ReactionReq req)
        {
            try
            {
                return await _reactionService.ToggleBookmark(req.PostId, user_unique_id);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "ToggleLike")]
        public async Task<JsonResponse> ToggleLike(LikeReq req)
        {
            try
            {
                return await _reactionService.ToggleLike(req.PostId,user_unique_id);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "GetAllCommnets")]
        public async Task<JsonResponse> GetAllCommnets(CommentReq req)
        {
            try
            {
                return await _reactionService.GetAllComments((int)req.PostId);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "InsertComment")]
        public async Task<JsonResponse> InsertComment(CommentInsertModel req)
        {
            try
            {
                return await _reactionService.InsertCommentAsync(req, username);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }
    }
}
