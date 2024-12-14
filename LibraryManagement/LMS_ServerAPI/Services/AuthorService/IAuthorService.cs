using LMS_ServerAPI.Models;

namespace LMS_ServerAPI.Services.AuthorService
{
    public interface IAuthorService
    {
        Task<IEnumerable<Author>> GetAuthors();
    }
}
