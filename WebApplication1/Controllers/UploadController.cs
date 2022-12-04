using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using WebApplication1.Helper;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class UploadController : Controller
    {
        private readonly DbaseContext _dbContext;
        private readonly IConfiguration _configuration;
        public UploadController(DbaseContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        [HttpGet]
        public ActionResult UploadDocument()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> UploadDocument(DocumentModel model, IFormFile file)
        {
            UploadHelper helper = new UploadHelper(_configuration);
            if (file != null)
            {
                string folder = "NepalLife\\WebApplication1\\WebApplication1\\Document";
                model.DocURL = await helper.UploadImage(folder, file);

            }
            await _dbContext.AddRangeAsync(model);

            return Ok();
        }
    }
}
