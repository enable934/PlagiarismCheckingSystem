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
        private readonly IStreamFetcher _streamFetcher;
        private readonly ICharacterEncoder _encoder;
        private readonly IPlagiarismDetector _plagiarismDetector;
        private readonly UnitOfWork _unitOfWork;

        public LaboratoryWorksController(ICharacterEncoder encoder, IPlagiarismDetector plagiarismDetector, UnitOfWork unitOfWork)
        {
            _streamFetcher = IOCContainer.Resolve<IStreamFetcher>();
            _encoder = encoder;
            _plagiarismDetector = plagiarismDetector;
            _unitOfWork = unitOfWork;
        }

        // GET: LaboratoryWorks
        public async Task<IActionResult> Index()
        {
            return View(_unitOfWork.LaboratoryWorkRepository.Get(filter: laboratoryWork => (laboratoryWork.User.Email == User.Identity.Name)));
        }

        // GET: LaboratoryWorks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var laboratoryWork = _unitOfWork.LaboratoryWorkRepository.Get(filter: m => m.Id == id).FirstOrDefault();
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

            var file = _unitOfWork.FileRepository.Get(filter: m => m.Id == id).FirstOrDefault();
            var anotherFile = _unitOfWork.FileRepository.Get(filter: m => m.Id == anotherId).FirstOrDefault();
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
                laboratoryWork.User = _unitOfWork.UserRepository.Get(filter: user => (user.Email == User.Identity.Name)).FirstOrDefault();
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
                _unitOfWork.LaboratoryWorkRepository.Insert(laboratoryWork);
                _unitOfWork.Save();
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

            var laboratoryWork = _unitOfWork.LaboratoryWorkRepository.Get(filter: laboratoryWork => (laboratoryWork.Id == id && laboratoryWork.User.Email == User.Identity.Name)).FirstOrDefault();
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
                    _unitOfWork.LaboratoryWorkRepository.Update(laboratoryWork);
                    _unitOfWork.Save();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_unitOfWork.LaboratoryWorkRepository.IsExists(laboratoryWork => laboratoryWork.Id == laboratoryWork.Id))
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

            var laboratoryWork = _unitOfWork.LaboratoryWorkRepository
                .Get(filter: m => m.Id == id).FirstOrDefault();
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
            var laboratoryWork = _unitOfWork.LaboratoryWorkRepository.GetByID(id);
            _unitOfWork.LaboratoryWorkRepository.Delete(laboratoryWork);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }


    }
}
