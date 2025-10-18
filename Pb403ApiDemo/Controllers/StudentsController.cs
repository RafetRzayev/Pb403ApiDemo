using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Pb403ApiDemo.DataContext;
using Pb403ApiDemo.Models;

namespace Pb403ApiDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public StudentsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var students = _dbContext.Students.ToList();

            return Ok(students);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var student = _dbContext.Students.FirstOrDefault(s => s.Id == id);

            if (student == null)
                return NotFound();
            return Ok(student);
        }

        [HttpPost]
        public IActionResult Create([FromForm]Student student)
        {
            _dbContext.Students.Add(student);
            _dbContext.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = student.Id }, student);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Student updatedStudent)
        {
            var student = _dbContext.Students.FirstOrDefault(s => s.Id == id);
            if (student == null)
                return NotFound();
            student.Name = updatedStudent.Name;
            _dbContext.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var student = _dbContext.Students.FirstOrDefault(s => s.Id == id);
            if (student == null)
                return NotFound();
            _dbContext.Students.Remove(student);
            _dbContext.SaveChanges();
            return NoContent();
        }

        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");
            
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            var filePath = Path.Combine(uploadsFolder, file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return Ok(new { filePath });
        }

        [HttpPost]
        [Route("download")]
        public IActionResult DownloadFile([FromQuery] string fileName)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            var filePath = Path.Combine(uploadsFolder, fileName);
            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found.");

            var contentTypeProvider = new FileExtensionContentTypeProvider();
            string contentType;
         
            if (!contentTypeProvider.TryGetContentType(fileName, out contentType))
            {
               contentType = "application/octet-stream";
            }

            var bytes = System.IO.File.ReadAllBytes(filePath);
            return File(bytes, contentType, "example");
        }
    }
}
