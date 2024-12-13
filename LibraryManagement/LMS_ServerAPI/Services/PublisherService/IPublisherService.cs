using LMS_ServerAPI.Models;

namespace LMS_ServerAPI.Services.PublisherService
{
    public interface IPublisherService
    {
        Task<IEnumerable<Publisher>> GetPublishers();
        Task<IEnumerable<Publisher>> GetPublishersNoParent();
        Task<IEnumerable<Publisher>> GetPublishersChild(string id);
    }
}
