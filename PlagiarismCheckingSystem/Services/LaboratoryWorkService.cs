using AutoMapper;
using Microsoft.AspNetCore.Http;
using PlagiarismCheckingSystem.Models;
using PlagiarismCheckingSystem.Repository;
using PlagiarismCheckingSystem.Util;
using PlagiarismCheckingSystem.ViewModels;
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
        private readonly IMapper _mapper;
        public LaboratoryWorkService(UnitOfWork unitOfWork, UserService userService, ICharacterEncoder encoder, IPlagiarismDetector plagiarismDetector, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _streamFetcher = IOCContainer.Resolve<IStreamFetcher>();
            _encoder = encoder;
            _plagiarismDetector = plagiarismDetector;
            _mapper = mapper;
        }

        public IEnumerable<LaboratoryWorkModel> GetLaboratoryWorksByUser(string userName)
        {
            return _mapper.Map<LaboratoryWork[], IEnumerable<LaboratoryWorkModel>>(_unitOfWork.LaboratoryWorkRepository.Get(filter: laboratoryWork => laboratoryWork.User.Email == userName).ToArray());
        }

        public LaboratoryWorkModel? GetLaboratoryWorkById(int? id)
        {
            return _mapper.Map<LaboratoryWorkModel>(_unitOfWork.LaboratoryWorkRepository.Get(filter: m => m.Id == id).FirstOrDefault());
        }


        public LaboratoryWorkModel? GetLaboratoryWorkByIdAndUser(int? id, string name)
        {
            return _mapper.Map<LaboratoryWorkModel>(_unitOfWork.LaboratoryWorkRepository.Get(filter: laboratoryWork => laboratoryWork.Id == id && laboratoryWork.User.Email == name).FirstOrDefault());
        }

        public void Create(LaboratoryWorkModel laboratoryWorkModel, string userName, List<IFormFile> files)
        {
            LaboratoryWork laboratoryWork = _mapper.Map<LaboratoryWork>(laboratoryWorkModel);
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

        public void Update(LaboratoryWorkModel laboratoryWorkModel)
        {
            var laboratoryWork = _unitOfWork.LaboratoryWorkRepository.GetByID(laboratoryWorkModel.Id);
            laboratoryWork.Title = laboratoryWorkModel.Title;
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
            return _unitOfWork.LaboratoryWorkRepository.IsExists(laboratoryWork => laboratoryWork.Id == id);
        }
    }
}
