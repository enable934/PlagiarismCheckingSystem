using Microsoft.AspNetCore.Http;
using PlagiarismCheckingSystem.Models;
using PlagiarismCheckingSystem.Repository;
using PlagiarismCheckingSystem.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlagiarismCheckingSystem.Services
{
    public class LaboratoryWorkService
    {
        private UnitOfWork _unitOfWork;
        private UserService _userService;
        private readonly IStreamFetcher _streamFetcher;
        private readonly ICharacterEncoder _encoder;
        private readonly IPlagiarismDetector _plagiarismDetector;
        public LaboratoryWorkService(UnitOfWork unitOfWork, UserService userService, ICharacterEncoder encoder, IPlagiarismDetector plagiarismDetector)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _streamFetcher = IOCContainer.Resolve<IStreamFetcher>();
            _encoder = encoder;
            _plagiarismDetector = plagiarismDetector;
        }

        public IEnumerable<LaboratoryWork> GetLaboratoryWorksByUser(string userName)
        {
            return _unitOfWork.LaboratoryWorkRepository.Get(filter: laboratoryWork => laboratoryWork.User.Email == userName);
        }

        public LaboratoryWork? GetLaboratoryWorkById(int? id)
        {
            return _unitOfWork.LaboratoryWorkRepository.Get(filter: m => m.Id == id).FirstOrDefault();
        }


        public LaboratoryWork? GetLaboratoryWorkByIdAndUser(int? id, string name)
        {
            return _unitOfWork.LaboratoryWorkRepository.Get(filter: laboratoryWork => laboratoryWork.Id == id && laboratoryWork.User.Email == name).FirstOrDefault();
        }

        public void Create(LaboratoryWork laboratoryWork, string userName, List<IFormFile> files)
        {
            laboratoryWork.User = _userService.GetUser(userName);
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
        }

        public void Update(LaboratoryWork laboratoryWork)
        {
            _unitOfWork.LaboratoryWorkRepository.Update(laboratoryWork);
            _unitOfWork.Save();
        }

        public void Delete(int id)
        {
            var laboratoryWork = _unitOfWork.LaboratoryWorkRepository.GetByID(id);
            _unitOfWork.LaboratoryWorkRepository.Delete(laboratoryWork);
            _unitOfWork.Save();
        }

        public bool IsExists(int id)
        {
            return _unitOfWork.LaboratoryWorkRepository.IsExists(laboratoryWork => laboratoryWork.Id == laboratoryWork.Id);
        }
    }
}
