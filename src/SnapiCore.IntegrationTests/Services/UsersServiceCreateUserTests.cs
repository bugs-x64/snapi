using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SnapiCore.Services;
using Xunit;

namespace SnapiCore.IntegrationTests.Services
{
    public class UsersServiceCreateUserTests
    {
        [Fact]
        public async Task CreateUser_NotExistsName_Created()
        {
            await using var context = DbContextFactory.Get();
            var service = new UsersService(context);
            var name = Guid.NewGuid().ToString();
            
            var result = await service.CreateUserAsync(name);
            var user = await context.Users.FirstOrDefaultAsync(x => x.Name == name);

            Assert.Equal(name, user?.Name);
            Assert.Equal(CreateUserStatus.Created, result.Status);
            Assert.NotNull(result.Value);
            Assert.Equal(name, result.Value.Name);
        }
        
        
        [Fact]
        public async Task CreateUser_EmptyName_TooShortName()
        {
            await using var context = DbContextFactory.Get();
            var service = new UsersService(context);
            var name = string.Empty;
            
            var result = await service.CreateUserAsync(name);

            Assert.Equal(CreateUserStatus.TooShortName, result.Status);
            Assert.Null(result.Value);
        }
        
        
        [Fact]
        public async Task CreateUser_NullName_TooShortName()
        {
            await using var context = DbContextFactory.Get();
            var service = new UsersService(context);
            string name = null;
            
            var result = await service.CreateUserAsync(name);

            Assert.Equal(CreateUserStatus.TooShortName, result.Status);
            Assert.Null(result.Value);
        }
        
        [Fact]
        public async Task CreateUser_ShortName_TooShortName()
        {
            await using var context = DbContextFactory.Get();
            var service = new UsersService(context);
            var name = "go";
            
            var result = await service.CreateUserAsync(name);

            Assert.Equal(CreateUserStatus.TooShortName, result.Status);
            Assert.Null(result.Value);
        }
        
        [Fact]
        public async Task CreateUser_LongName_TooLongName()
        {
            await using var context = DbContextFactory.Get();
            var service = new UsersService(context);
            var name = string.Join("",Enumerable.Range(0, 65));
            
            var result = await service.CreateUserAsync(name);

            Assert.Equal(CreateUserStatus.TooLongName, result.Status);
            Assert.Null(result.Value);
        }
        
        [Fact]
        public async Task CreateUser_ExistsName_AlreadyExists()
        {
            await using var context = DbContextFactory.Get();
            var service = new UsersService(context);
            var name = Guid.NewGuid().ToString();
            
            await service.CreateUserAsync(name);
            var result = await service.CreateUserAsync(name);

            Assert.Equal(CreateUserStatus.AlreadyExists, result.Status);
            Assert.Null(result.Value);
        }
        
        
    }
}