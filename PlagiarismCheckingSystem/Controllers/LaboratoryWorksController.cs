using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiffMatchPatch;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlagiarismCheckingSystem.Data;
using PlagiarismCheckingSystem.Models;
using PlagiarismCheckingSystem.Services;
using File = PlagiarismCheckingSystem.Models.File;

namespace PlagiarismCheckingSystem.Controllers
{
    [Authorize]
    public class LaboratoryWorksController : Controller
    {
        private readonly Context _context;
        private readonly IStreamFetcher _streamFetcher;
        private readonly ICharacterEncoder _encoder;
        private readonly IPlagiarismDetector _plagiarismDetector;

        public LaboratoryWorksController(Context context, IStreamFetcher streamFetcher, ICharacterEncoder encoder, IPlagiarismDetector plagiarismDetector)
        {
            _context = context;
            _streamFetcher = streamFetcher;
            _encoder = encoder;
            _plagiarismDetector = plagiarismDetector;
        }

        // GET: LaboratoryWorks
        public async Task<IActionResult> Index()
        {
            return View(await _context.LaboratoryWork.Where(laboratoryWork => (laboratoryWork.User.Email == User.Identity.Name)).ToListAsync());
        }

        // GET: LaboratoryWorks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var laboratoryWork = await _context.LaboratoryWork
                .FirstOrDefaultAsync(m => m.Id == id);
            if (laboratoryWork == null)
            {
                return NotFound();
            }

            return View(laboratoryWork);
        }



        // GET: LaboratoryWorks/DetailsFile/5?anotherId=20
        public async Task<IActionResult> DetailsFile(int? id, int? anotherId)
        {
            if (id == null || anotherId == null)
            {
                return NotFound();
            }

            var file = await _context.Files
                .FirstOrDefaultAsync(m => m.Id == id);
            var anotherFile = await _context.Files
                .FirstOrDefaultAsync(m => m.Id == anotherId);
            if (file == null || anotherFile == null)
            {
                return NotFound();
            }
            ViewData["diff"] = _plagiarismDetector.GeneratePrettyHtmlDiff(file.Content, anotherFile.Content);

            return View();
        }

        // GET: LaboratoryWorks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LaboratoryWorks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title")] LaboratoryWork laboratoryWork, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                laboratoryWork.User = await _context.Users.FirstOrDefaultAsync(user => (user.Email == User.Identity.Name));
                foreach (var item in files)
                {
                    if (item.Length > 0)
                    {
                        var stream = _streamFetcher.GetArray(item);
                        var file = new File
                        {
                            Content = _encoder.Encode(stream),
                            Name = item.FileName,
                            LaboratoryWork = laboratoryWork
                        };
                        laboratoryWork.Files.Add(file);
                    }
                }
                Dictionary<LaboratoryWork, List<Similarity>> similarities = _plagiarismDetector.FindAndSaveSimilarities(laboratoryWork);
                decimal plagiarismValue = _plagiarismDetector.CalculatePlagiarism(similarities);
                laboratoryWork.Similarity = plagiarismValue;
                _context.Add(laboratoryWork);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(laboratoryWork);
        }

        // GET: LaboratoryWorks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var laboratoryWork = await _context.LaboratoryWork.FirstOrDefaultAsync(laboratoryWork => (laboratoryWork.Id == id && laboratoryWork.User.Email == User.Identity.Name));
            if (laboratoryWork == null)
            {
                return NotFound();
            }
            return View(laboratoryWork);
        }

        // POST: LaboratoryWorks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title")] LaboratoryWork laboratoryWork)
        {
            if (id != laboratoryWork.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(laboratoryWork);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LaboratoryWorkExists(laboratoryWork.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(laboratoryWork);
        }

        // GET: LaboratoryWorks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var laboratoryWork = await _context.LaboratoryWork
                .FirstOrDefaultAsync(m => m.Id == id);
            if (laboratoryWork == null)
            {
                return NotFound();
            }

            return View(laboratoryWork);
        }

        // POST: LaboratoryWorks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var laboratoryWork = await _context.LaboratoryWork.FindAsync(id);
            _context.LaboratoryWork.Remove(laboratoryWork);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LaboratoryWorkExists(int id)
        {
            return _context.LaboratoryWork.Any(e => e.Id == id);
        }
    }
}
