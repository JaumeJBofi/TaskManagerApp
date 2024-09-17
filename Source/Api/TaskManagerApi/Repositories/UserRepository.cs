using MongoDB.Driver;
using TaskManagerApi.Models;
using TaskManagerApi.Repositories.Interfaces;
using TaskManagerApi.Models.Dtos.Authentication;
using MongoDB.Bson;
using TaskManagerApi.Services.Interfaces;
using TaskManagerApi.Services;
using TaskManagerApi.Utilities;

namespace TaskManagerApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IConfig _config;
        private readonly IPasswordService _passwordService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IMongoCollection<User> _usersCollection;        

        public UserRepository(IConfig config, IMongoDatabase mongoDatabase, IPasswordService passwordService, IJwtTokenService jwtTokenService)
        {
            _config = config;
            _passwordService = passwordService;
            _jwtTokenService = jwtTokenService;            
            _usersCollection = mongoDatabase.GetCollection<User>("Users");            
        }

        public async Task<List<User>> GetAllAsync()
        {            
            return await _usersCollection.Find(_ => true).ToListAsync();            
        }
      
        public async Task<User> CreateAsync(SignInDto newUserDto)
        {
            if (String.IsNullOrEmpty(newUserDto.UserName)) throw new Exception("Username can't be empty");
            string passwordHash = _passwordService.HashPassword(newUserDto.Password);
            
            var newUser = new User
            {
                Id = ObjectId.GenerateNewId().ToString(), 
                UserName = newUserDto.UserName,
                Email = newUserDto.Email,
                PasswordHash = passwordHash                
            };

            await _usersCollection.InsertOneAsync(newUser);
            return newUser;
        }

        public async Task<int> DeleteAsync(string id)
        {
            var deleteResult = await _usersCollection.DeleteOneAsync(x => x.Id == id);
            return (int)deleteResult.DeletedCount;            
        }

        public Task DeleteByUserNameAsync(string userName)
        {
            return  _usersCollection.DeleteOneAsync(x => x.UserName == userName);
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            return await _usersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<User?> GetByUserNameAsync(string userName)
        {
            return await _usersCollection.Find(x => x.UserName == userName).FirstOrDefaultAsync();            
        }      

        public async Task<DateTime> UpdateUserRefreshToken(string userName)
        {
            var currentTime = DateTime.Now;
            var expireDate = DateTime.Now.AddMinutes(_config.JwtSettings.RefreshTokenExpirationMinutes);

            var updateResult = await _usersCollection.UpdateOneAsync(
                filter: x => x.UserName == userName,
                update: Builders<User>.Update.Set(x => x.RefreshToken, _jwtTokenService.GenerateRefreshToken()).Set(x => x.RefreshTokenExpiry, expireDate)
            );

            return expireDate;
        }

        public async Task<bool> RevokeRefreshToken(string userName,string refreshToken)
        {
            var user = await GetByUserNameAsync(userName);
            if (user == null || user.RefreshToken == refreshToken) return false;

            var filter = Builders<User>.Filter.Eq(x => x.UserName, userName);
            var update = Builders<User>.Update.Set(x => x.RefreshToken, null).Set(x => x.RefreshTokenExpiry, null);
            var updateResult = await _usersCollection.UpdateOneAsync(filter, update);

            return updateResult.ModifiedCount > 0;
        }

        public async Task<bool> ValidateRefreshToken(string userName, string refreshToken)
        {
            var user = await GetByUserNameAsync(userName);
            return user != null && user.RefreshToken == refreshToken && user.RefreshTokenExpiry >= DateTime.Now;
        }
    }
}
