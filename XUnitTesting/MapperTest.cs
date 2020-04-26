using AutoMapper;
using PlagiarismCheckingSystem.Models;
using PlagiarismCheckingSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xunit;

namespace XUnitTesting
{
    public class MapperTest
    {
        [Fact]
        public void ValidMappingTest()
        {
            //Arrange
            var sourceObj = new RegisterModel {Email="@example.com",Password="123" };
            var config = new MapperConfiguration(cfg => cfg.CreateMap<RegisterModel, User>());
            var mapper = config.CreateMapper();

            //Act
            var destinationObj = mapper.Map<User>(sourceObj);

            //Assert
            Assert.True(destinationObj is User);
            Assert.Equal(sourceObj.Email, destinationObj.Email);
            Assert.Equal(sourceObj.Password, destinationObj.Password);
        }
    }
}
