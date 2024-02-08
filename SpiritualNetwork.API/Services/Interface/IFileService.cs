namespace SpiritualNetwork.API.Services.Interface
{
    public interface IFileService
    {
        public Task<List<Entities.File>> UploadFile(IFormCollection form);
    }
}
