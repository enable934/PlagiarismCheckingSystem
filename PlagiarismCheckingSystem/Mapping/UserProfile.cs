using AutoMapper;
using PlagiarismCheckingSystem.Models;
using PlagiarismCheckingSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlagiarismCheckingSystem.Mapping
{
    public class UserProfile: Profile
    {
        public UserProfile()
        {
            CreateMap<RegisterModel, User>();
        }
    }
}
