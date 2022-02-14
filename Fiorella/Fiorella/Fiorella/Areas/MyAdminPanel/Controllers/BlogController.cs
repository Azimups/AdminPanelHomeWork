using System;
using System.Threading.Tasks;
using Fiorella.DataLayerAccess;
using Fiorella.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fiorella.Areas.MyAdminPanel.Controllers
{
    [Area("MyAdminPanel")]
    public class BlogController : Controller
    {
        private readonly AppDbContext _dbContext;

        public BlogController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var bloggers = await _dbContext.Bloggers.ToListAsync();
            return View(bloggers);
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id==null)
            {
                return BadRequest();
            }

            var blogger = await _dbContext.Bloggers.FindAsync(id);
            if (blogger==null)
            {
                return NotFound();
            }

            return View(blogger);
        }

        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Blogger blogger)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var isExistBlogger = await _dbContext.Bloggers.AnyAsync(x => x.BloggerTitle.ToLower() == blogger.BloggerTitle.ToLower());
            if (isExistBlogger)
            {
                ModelState.AddModelError("BloggerTitle","Bu adda Blogger movcuddur");
                return View();
            }

            await _dbContext.Bloggers.AddAsync(blogger);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}