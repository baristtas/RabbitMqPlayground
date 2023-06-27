using CreateExcelFile.Hubs;
using CreateExcelFile.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CreateExcelFile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<MyHub> _hubContext;
        public FilesController(AppDbContext context,IHubContext<MyHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file, int fileId)
        {
            if (file is not { Length: > 0 }) return BadRequest();

            var userFile = await _context.UserFiles.FirstAsync(x => x.Id == fileId);

            var filePath = userFile.FileName + Path.GetExtension(file.FileName);

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\files", filePath);

            using FileStream stream = new(path, FileMode.Create);

            await file.CopyToAsync(stream);

            userFile.CreatedDate = DateTime.Now;
            userFile.FilePath = path;
            userFile.FileStatus = FileStatus.Completed;

            await _context.SaveChangesAsync();

            //signalr notfiitcation olustuurulacak.
            await _hubContext.Clients.User(userFile.UserId).SendAsync("CompletedFile");

            return Ok();
        }

        [HttpGet("{fileId}")]
        public async Task<IActionResult> Download(int fileId)
        {
            if(!HttpContext.User.Identity.IsAuthenticated) return Unauthorized("Unauthorized action is not allowed.");
            var fileEntity = await _context.UserFiles.FirstOrDefaultAsync(x => x.Id == fileId && x.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (fileEntity == null) return BadRequest("File not found or you have not access to the file");
        
            var filePath = fileEntity.FilePath;
        
        
            var file = System.IO.File.ReadAllBytes(filePath);
        
            if (file.Length <= 0) return BadRequest("File not found");
        
        
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileEntity.FileName + Path.GetExtension(filePath));
        }

    }
}
