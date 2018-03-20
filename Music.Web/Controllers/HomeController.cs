using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Music.Core.Interfaces;
using Music.Infrastructure.Repository;

namespace Music.Web.Controllers
{
	[AllowAnonymous]
	[RoutePrefix("")]
	public class HomeController : Controller
	{
		private readonly IGenreRepository _genreRepository;
		private readonly IBandRepository _bandRepository;

		public HomeController()
			:this(new GenreRepository(),new BandRepository())
		{
		}

		public HomeController(IGenreRepository genreRepository,IBandRepository bandRepository)
		{
			_genreRepository = genreRepository;
			_bandRepository = bandRepository;
		}


        // GET: Home
        public async Task<ActionResult> Index()
        {
	        ViewBag.ListGenres = await _genreRepository.GetAllyAsync();

            return View();
        }
    }
}