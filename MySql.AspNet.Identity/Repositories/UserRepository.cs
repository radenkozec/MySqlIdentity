using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Linq;

namespace MySql.AspNet.Identity.Repositories
{
    public class UserRepository<TUser> where TUser : IdentityUser
    {
        private readonly string _connectionString;
        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Insert(TUser user)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                var parameters = new Dictionary<string, object>
                {
                    {"@Id", user.Id},
                    {"@Email", (object) user.Email ?? DBNull.Value},
                    {"@EmailConfirmed", user.EmailConfirmed},
                    {"@PasswordHash", (object) user.PasswordHash ?? DBNull.Value},
                    {"@SecurityStamp", (object) user.SecurityStamp ?? DBNull.Value},
                    {"@PhoneNumber", (object) user.PhoneNumber ?? DBNull.Value},
                    {"@PhoneNumberConfirmed", user.PhoneNumberConfirmed},
                    {"@TwoFactorAuthEnabled", user.TwoFactorAuthEnabled},
                    {"@LockoutEndDate", (object) user.LockoutEndDate ?? DBNull.Value},
                    {"@LockoutEnabled", user.LockoutEnabled},
                    {"@AccessFailedCount", user.AccessFailedCount},
                    {"@UserName", user.UserName}
                };

                MySqlHelper.ExecuteNonQuery(conn, @"INSERT INTO aspnetusers VALUES(@Id,@Email,@EmailConfirmed,@PasswordHash,@SecurityStamp,@PhoneNumber,@PhoneNumberConfirmed,
                @TwoFactorAuthEnabled,@LockoutEndDate,@LockoutEnabled,@AccessFailedCount,@UserName)", parameters);
            }
        }

        public void Delete(TUser user)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                var parameters = new Dictionary<string, object>
                {
                    {"@Id", user.Id}
                };

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM aspnetusers WHERE Id=@Id", parameters);
            }
        }

        public IQueryable<TUser> GetAll()
        {
            List<TUser> users = new List<TUser>();

            using (var conn = new MySqlConnection(_connectionString))
            {
                var reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                    @"SELECT Id,Email,EmailConfirmed,
                PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,
                LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName FROM aspnetusers", null);
                
                while (reader.Read())
                {
                    var user = (TUser)Activator.CreateInstance(typeof(TUser));
                    user.Id = reader[0].ToString();
                    user.Email = reader[1].ToString();
                    user.EmailConfirmed = (bool)reader[2];
                    user.PasswordHash = reader[3].ToString();
                    user.SecurityStamp = reader[4].ToString();
                    user.PhoneNumber = reader[5].ToString();
                    user.PhoneNumberConfirmed = (bool)reader[6];
                    user.TwoFactorAuthEnabled = (bool)reader[7];
                    user.LockoutEndDate = reader[8] == DBNull.Value ? null : (DateTime?)reader[8];
                    user.LockoutEnabled = (bool)reader[9];
                    user.AccessFailedCount = (int)reader[10];
                    user.UserName = reader[11].ToString();

                    users.Add(user);
                }

            }
            return users.AsQueryable<TUser>();
        }
        
        public TUser GetById(string userId)
        {
            var user = (TUser)Activator.CreateInstance(typeof(TUser));
            using (var conn = new MySqlConnection(_connectionString))
            {
                var parameters = new Dictionary<string, object>
                {
                    {"@Id", userId}
                };

                var reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                    @"SELECT Id,Email,EmailConfirmed,
                PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,
                LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName FROM aspnetusers WHERE Id=@Id", parameters);
                while (reader.Read())
                {
                    user.Id = reader[0].ToString();
                    user.Email = reader[1].ToString();
                    user.EmailConfirmed = (bool)reader[2];
                    user.PasswordHash = reader[3].ToString();
                    user.SecurityStamp = reader[4].ToString();
                    user.PhoneNumber = reader[5].ToString();
                    user.PhoneNumberConfirmed = (bool)reader[6];
                    user.TwoFactorAuthEnabled = (bool)reader[7];
                    user.LockoutEndDate = reader[8] == DBNull.Value ? null : (DateTime?)reader[8];
                    user.LockoutEnabled = (bool)reader[9];
                    user.AccessFailedCount = (int)reader[10];
                    user.UserName = reader[11].ToString();
                }

            }
            return user;
        }

        public TUser GetByName(string userName)
        {
            var user = (TUser)Activator.CreateInstance(typeof(TUser));
            using (var conn = new MySqlConnection(_connectionString))
            {
                var parameters = new Dictionary<string, object>
                {
                    {"@UserName", userName}
                };

                var reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                    @"SELECT Id,Email,EmailConfirmed,
                PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,
                LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName FROM aspnetusers WHERE UserName=@UserName", parameters);
                while (reader.Read())
                {
                    user.Id = reader[0].ToString();
                    user.Email = reader[1].ToString();
                    user.EmailConfirmed = (bool)reader[2];
                    user.PasswordHash = reader[3].ToString();
                    user.SecurityStamp = reader[4].ToString();
                    user.PhoneNumber = reader[5].ToString();
                    user.PhoneNumberConfirmed = (bool)reader[6];
                    user.TwoFactorAuthEnabled = (bool)reader[7];
                    user.LockoutEndDate = reader[8] == DBNull.Value ? null : (DateTime?)reader[8];
                    user.LockoutEnabled = (bool)reader[9];
                    user.AccessFailedCount = (int)reader[10];
                    user.UserName = reader[11].ToString();
                }

            }
            return user;
        }

        public TUser GetByEmail(string email)
        {
            var user = (TUser)Activator.CreateInstance(typeof(TUser));
            using (var conn = new MySqlConnection(_connectionString))
            {
                var parameters = new Dictionary<string, object>
                {
                    {"@Email", email}
                };

                var reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                    @"SELECT Id,Email,EmailConfirmed,
                PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,
                LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName FROM aspnetusers WHERE Email=@Email", parameters);
                while (reader.Read())
                {
                    user.Id = reader[0].ToString();
                    user.Email = reader[1].ToString();
                    user.EmailConfirmed = (bool)reader[2];
                    user.PasswordHash = reader[3].ToString();
                    user.SecurityStamp = reader[4].ToString();
                    user.PhoneNumber = reader[5].ToString();
                    user.PhoneNumberConfirmed = (bool)reader[6];
                    user.TwoFactorAuthEnabled = (bool)reader[7];
                    user.LockoutEndDate = reader[8] == DBNull.Value ? null : (DateTime?)reader[8];
                    user.LockoutEnabled = (bool)reader[9];
                    user.AccessFailedCount = (int)reader[10];
                    user.UserName = reader[11].ToString();
                }

            }
            return user;
        }

       

        public void Update(TUser user)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                var parameters = new Dictionary<string, object>
                {
                    {"@NewId", user.Id},
                    {"@Email", (object) user.Email ?? DBNull.Value},
                    {"@EmailConfirmed", user.EmailConfirmed},
                    {"@PasswordHash", (object) user.PasswordHash ?? DBNull.Value},
                    {"@SecurityStamp", (object) user.SecurityStamp ?? DBNull.Value},
                    {"@PhoneNumber", (object) user.PhoneNumber ?? DBNull.Value},
                    {"@PhoneNumberConfirmed", user.PhoneNumberConfirmed},
                    {"@TwoFactorAuthEnabled", user.TwoFactorAuthEnabled},
                    {"@LockoutEndDate", (object) user.LockoutEndDate ?? DBNull.Value},
                    {"@LockoutEnabled", user.LockoutEnabled},
                    {"@AccessFailedCount", user.AccessFailedCount},
                    {"@UserName", user.UserName},
                    {"@Id", user.Id}
                };

                MySqlHelper.ExecuteNonQuery(conn, @"UPDATE aspnetusers 
                SET Id = @NewId,Email=@Email,EmailConfirmed=@EmailConfirmed,PasswordHash=@PasswordHash,SecurityStamp=@SecurityStamp,PhoneNumber=@PhoneNumber,PhoneNumberConfirmed=@PhoneNumberConfirmed,
                TwoFactorEnabled=@TwoFactorAuthEnabled,LockoutEndDateUtc=@LockoutEndDate,LockoutEnabled=@LockoutEnabled,AccessFailedCount=@AccessFailedCount,UserName=@UserName
                WHERE Id=@Id", parameters);
            }
        }
    }
}
