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
using PlagiarismCheckingSystem.Models;
using PlagiarismCheckingSystem.Repository;
using PlagiarismCheckingSystem.Services;
using PlagiarismCheckingSystem.Util;
using File = PlagiarismCheckingSystem.Models.File;

namespace PlagiarismCheckingSystem.Controllers
{
    [Authorize]
    public class LaboratoryWorksController : Controller
    {
        private readonly LaboratoryWorkService _laboratoryWorkService;
        private readonly FileService _fileService;
        private readonly IPlagiarismDetector _plagiarismDetector;

        public LaboratoryWorksController(LaboratoryWorkService laboratoryWorkService, FileService fileService, IPlagiarismDetector plagiarismDetector)
        {
            
            _laboratoryWorkService = laboratoryWorkService;
            _fileService = fileService;
            _plagiarismDetector = plagiarismDetector;
        }

        // GET: LaboratoryWorks
        public async Task<IActionResult> Index()
        {
            return View(_laboratoryWorkService.GetLaboratoryWorksByUser(User.Identity.Name));
        }

        // GET: LaboratoryWorks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var laboratoryWork = _laboratoryWorkService.GetLaboratoryWorkById(id);
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

            var file = _fileService.GetFileById(id);
            var anotherFile = _fileService.GetFileById(anotherId);
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
                _laboratoryWorkService.Create(laboratoryWork, User.Identity.Name, files);
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

            var laboratoryWork = _laboratoryWorkService.GetLaboratoryWorkByIdAndUser(id, User.Identity.Name);
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
                    _laboratoryWorkService.Update(laboratoryWork);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_laboratoryWorkService.IsExists(id))
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

            var laboratoryWork = _laboratoryWorkService.GetLaboratoryWorkById(id);
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
            _laboratoryWorkService.Delete(id);
            return RedirectToAction(nameof(Index));
        }


    }
}
