using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpiritualNetwork.API.Helper;
using SpiritualNetwork.API.Services;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FileController : ApiBaseController
    {
        private readonly IFileService _fileService;
        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost(Name = "UploadFile")]
        public async Task<JsonResponse> UploadFile(IFormCollection form)
        {
            try
            {
                var response = await _fileService.UploadFile(form);
                return new JsonResponse(200, true, "success", response);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet(Name = "StreamVideo")]
        public async Task<IActionResult> StreamVideo(string videoPath)
        {
            int ChunkSize = 1024 * 1024; // Set your desired chunk size (e.g., 1 MB)

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(videoPath, HttpCompletionOption.ResponseHeadersRead);

                if (!response.IsSuccessStatusCode)
                {
                    return NotFound();
                }

                var stream = await response.Content.ReadAsStreamAsync();
                return File(new VideoStream(stream, ChunkSize), response.Content.Headers.ContentType?.MediaType);
            }
            //var stream = new FileStream(videoPath, FileMode.Open, FileAccess.Read);
            //return File(stream, "video/mp4");
        }
        
    }
}
