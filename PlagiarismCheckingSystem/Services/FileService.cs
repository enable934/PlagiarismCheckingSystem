using PlagiarismCheckingSystem.Models;
using PlagiarismCheckingSystem.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlagiarismCheckingSystem.Services
{
    public class FileService
    {
        private UnitOfWork _unitOfWork;
        public FileService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public File? GetFileById(int? id)
        {
            return _unitOfWork.FileRepository.Get(filter: m => m.Id == id).FirstOrDefault();
        }
    }
}
