using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using WebApplication1.Helper;
using WebApplication1.Data;
using WebApplication1.Models;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize]
        public ActionResult UploadDocument()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> UploadDocument(IFormFile file)
        {
            UploadHelper helper = new UploadHelper(_configuration);
            DocumentModel model = new DocumentModel();
            try
            {
                if (file != null)
                {
                    string folder = "NepalLife\\WebApplication1\\WebApplication1\\Document";
                    model.DocURL = await helper.UploadImage(folder, file);

                }
                await _dbContext.Documents.AddRangeAsync(model);
                _dbContext.SaveChanges();
                ViewBag.Message = "Successfully Uploaded";
            }
            catch(Exception ex)
            {
                ViewBag.Message = "Upload failed. Try again.";
            }

            return View();
        }
    }
}
