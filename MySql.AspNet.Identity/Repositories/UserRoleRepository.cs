using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace MySql.AspNet.Identity.Repositories
{
    public class UserRoleRepository<TUser> where TUser:IdentityUser
    {
        private readonly string _connectionString;
        public UserRoleRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Insert(TUser user, string roleName)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                var parameters = new Dictionary<string, object>
                {
                    {"@RoleName", roleName}
                };

                var idObject = MySqlHelper.ExecuteScalar(conn, CommandType.Text,
                    @"SELECT Id FROM aspnetroles WHERE Name=@RoleName", parameters);
                string roleId = idObject == null ? null : idObject.ToString();


                if (!string.IsNullOrEmpty(roleId))
                {

                    using (var conn1 = new MySqlConnection(_connectionString))
                    {
                        var parameters1 = new Dictionary<string, object>
                        {
                            {"@Id", user.Id},
                            {"@RoleId", roleId}
                        };

                        MySqlHelper.ExecuteNonQuery(conn1, @"Insert into aspnetuserroles(UserId,RoleId) VALUES(@Id,@RoleId)", parameters1);
                    }
                }
            }
        }

        public void Delete(TUser user, string roleName)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                var parameters = new Dictionary<string, object>
                {
                    {"@RoleName", roleName}
                };

                var idObject = MySqlHelper.ExecuteScalar(conn, CommandType.Text,
                    @"SELECT Id FROM aspnetroles WHERE Name=@RoleName", parameters);
                string roleId = idObject == null ? null : idObject.ToString();


                if (!string.IsNullOrEmpty(roleId))
                {

                    using (var conn1 = new MySqlConnection(_connectionString))
                    {
                        var parameters1 = new Dictionary<string, object>
                        {
                            {"@Id", user.Id},
                            {"@RoleId", roleId}
                        };

                        MySqlHelper.ExecuteNonQuery(conn1, @"Delete FROM aspnetuserroles WHERE UserId=@Id AND RoleId=@RoleId", parameters1);
                    }
                }
            }
        }

        public List<String> PopulateRoles(string userId)
        {
            var roleIds = new List<string>();
            var listRoles = new List<string>();
            using (var conn = new MySqlConnection(_connectionString))
            {
                var parameters = new Dictionary<string, object>
                {
                    {"@Id", userId}
                };

                // we do select 2 times instead of join for compatibility
                // we probobly wan't have users which has more than 2 or 3 roles in the same time 


                var reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                    @"SELECT RoleId FROM aspnetuserroles Where UserId=@Id", parameters);
                while (reader.Read())
                {
                    roleIds.Add(reader[0].ToString());
                }

            }

            foreach (var roleId in roleIds)
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    var parameters = new Dictionary<string, object>
                {
                    {"@Id", roleId}
                };

                    var reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                        @"SELECT Name FROM aspnetroles Where Id=@Id", parameters);
                    while (reader.Read())
                    {
                        listRoles.Add(reader[0].ToString());
                    }

                }
            }

            return listRoles;

        }
    }
}
