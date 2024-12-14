using LMS_ServerAPI.Models;
using LMS_ServerAPI.Repositories.PublisherRepository;
using LMS_ServerAPI.Services.AuthorService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS_ServerAPI.Services.PublisherService
{

	public class PublisherService : IPublisherService
	{
		private readonly IPublisherRepository _repo;
		public PublisherService(IPublisherRepository repo)
		{
			_repo = repo;
		}

		public async Task<IEnumerable<Publisher>> GetPublishers()
		{
			try
			{
				var publishers = await _repo.GetAll();
				return publishers;
			}
			catch
			{
				return null!;
			}
		}

		public async Task<IEnumerable<Publisher>> GetPublishersChild(string id)
		{
			int idInterger = int.Parse(id);
			var childs = await  _repo.GetChild(idInterger);
			return childs;
		}

		public async Task<IEnumerable<Publisher>> GetPublishersNoParent()
		{
			var publishers = await _repo.GetNoParent();
			return publishers;
		}
	}
}
