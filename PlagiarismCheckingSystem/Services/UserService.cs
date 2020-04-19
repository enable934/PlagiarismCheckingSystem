using PlagiarismCheckingSystem.Models;
using PlagiarismCheckingSystem.Repository;
using PlagiarismCheckingSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlagiarismCheckingSystem.Services
{
    public class UserService
    {
        private UnitOfWork _unitOfWork;
        public UserService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public User? GetUser(string name)
        {
            return _unitOfWork.UserRepository.Get(filter: user => user.Email == name).FirstOrDefault();
        }

        public User? GetUserByEmailAndLogin(string email, string password)
        {
            return _unitOfWork.UserRepository.Get(filter: user => user.Email == email && user.Password == password).FirstOrDefault();
        }

        public void Register(RegisterModel model)
        {
            _unitOfWork.UserRepository.Insert(new User { Email = model.Email, Password = model.Password });
            _unitOfWork.Save();
        }
    }
}
