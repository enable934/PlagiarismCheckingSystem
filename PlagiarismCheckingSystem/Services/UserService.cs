using AutoMapper;
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
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UserService(UnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
            var user = _mapper.Map<User>(model);
            _unitOfWork.UserRepository.Insert(user);
            _unitOfWork.Save();
        }
    }
}
