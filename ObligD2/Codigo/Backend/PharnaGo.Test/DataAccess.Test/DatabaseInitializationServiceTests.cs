using Microsoft.EntityFrameworkCore;
using Moq;
using PharmaGo.DataAccess.Services;
using PharmaGo.Domain.Entities;
using PharmaGo.IDataAccess;

namespace PharmaGo.Test.DataAccess.Test
{
    [TestClass]
    public class DatabaseInitializationServiceTests
    {
        private Mock<IRepository<User>> _userRepositoryMock = null!;
        private Mock<IRepository<Role>> _roleRepositoryMock = null!;
        private DatabaseInitializationService _databaseInitializationService = null!;

        [TestInitialize]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IRepository<User>>();
            _roleRepositoryMock = new Mock<IRepository<Role>>();
            _databaseInitializationService = new DatabaseInitializationService(
                _userRepositoryMock.Object, 
                _roleRepositoryMock.Object);
        }

        [TestMethod]
        public void Initialize_WhenNoAdminUserExists_ShouldCreateAdminUser()
        {
            // Arrange
            var adminRole = new Role { Name = "Administrator" };

            _roleRepositoryMock.Setup(r => r.GetOneByExpression(It.IsAny<System.Linq.Expressions.Expression<System.Func<Role, bool>>>()))
                .Returns(adminRole);
            _roleRepositoryMock.Setup(r => r.Save());

            _userRepositoryMock.Setup(u => u.GetOneDetailByExpression(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>()))
                .Returns(default(User));
            _userRepositoryMock.Setup(u => u.InsertOne(It.IsAny<User>()));
            _userRepositoryMock.Setup(u => u.Save());

            // Act
            _databaseInitializationService.Initialize();

            // Assert
            _userRepositoryMock.Verify(u => u.InsertOne(It.Is<User>(user => 
                user.UserName == "admin" && 
                user.Email == "admin@pharmago.com" && 
                user.Password == "Admin123!")), Times.Once);
            _userRepositoryMock.Verify(u => u.Save(), Times.Once);
        }

        [TestMethod]
        public void Initialize_WhenAdminUserAlreadyExists_ShouldNotCreateAdminUser()
        {
            // Arrange
            var adminRole = new Role { Name = "Administrator" };
            var existingAdminUser = new User 
            { 
                UserName = "existingAdmin", 
                Email = "existing@test.com",
                Password = "password",
                Address = "test",
                RegistrationDate = DateTime.Now,
                Role = adminRole 
            };

            _roleRepositoryMock.Setup(r => r.GetOneByExpression(It.IsAny<System.Linq.Expressions.Expression<System.Func<Role, bool>>>()))
                .Returns(adminRole);
            _roleRepositoryMock.Setup(r => r.Save());

            _userRepositoryMock.Setup(u => u.GetOneDetailByExpression(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>()))
                .Returns(existingAdminUser);

            // Act
            _databaseInitializationService.Initialize();

            // Assert
            _userRepositoryMock.Verify(u => u.InsertOne(It.IsAny<User>()), Times.Never);
            _userRepositoryMock.Verify(u => u.Save(), Times.Never);
        }

        [TestMethod]
        public void Initialize_WhenNoRolesExist_ShouldCreateAllRoles()
        {
            // Arrange
            _roleRepositoryMock.SetupSequence(r => r.GetOneByExpression(It.IsAny<System.Linq.Expressions.Expression<System.Func<Role, bool>>>()))
                .Returns((Role)null!) // Administrator doesn't exist
                .Returns((Role)null!) // Employee doesn't exist  
                .Returns((Role)null!) // Owner doesn't exist
                .Returns(new Role { Name = "Administrator" }); // Admin exists for user creation

            _roleRepositoryMock.Setup(r => r.InsertOne(It.IsAny<Role>()));
            _roleRepositoryMock.Setup(r => r.Save());

            _userRepositoryMock.Setup(u => u.GetOneDetailByExpression(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>()))
                .Returns(default(User));
            _userRepositoryMock.Setup(u => u.InsertOne(It.IsAny<User>()));
            _userRepositoryMock.Setup(u => u.Save());

            // Act
            _databaseInitializationService.Initialize();

            // Assert
            _roleRepositoryMock.Verify(r => r.InsertOne(It.IsAny<Role>()), Times.Exactly(3));
            _roleRepositoryMock.Verify(r => r.Save(), Times.Once);
            _userRepositoryMock.Verify(u => u.InsertOne(It.IsAny<User>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Initialize_WhenAdminRoleCannotBeCreated_ShouldThrowException()
        {
            // Arrange - Admin role never gets created
            _roleRepositoryMock.Setup(r => r.GetOneByExpression(It.IsAny<System.Linq.Expressions.Expression<System.Func<Role, bool>>>()))
                .Returns((Role)null!);
            _roleRepositoryMock.Setup(r => r.InsertOne(It.IsAny<Role>()));
            _roleRepositoryMock.Setup(r => r.Save());

            // Act
            _databaseInitializationService.Initialize();

            // Assert - Exception should be thrown
        }
    }
}