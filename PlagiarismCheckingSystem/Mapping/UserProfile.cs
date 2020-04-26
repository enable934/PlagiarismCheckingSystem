using AutoMapper;
using PlagiarismCheckingSystem.Models;
using PlagiarismCheckingSystem.ViewModels;

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
