    using LMS_ServerAPI.Models;
using LMS_ServerAPI.Repositories.AuthorRepositories;
using LMS_ServerAPI.Services.AuthorService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS_ServerAPI.Services.AuthorService
{

    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _repo;
        public AuthorService(IAuthorRepository repo)
        {
            _repo = repo;
        }

		public async Task<IEnumerable<Author>> GetAuthors()
		{
            try
            {
                var authors =  await _repo.GetAll();
                return authors;
            }
            catch 
            {
                return null!;
            }

		}

	}
}
