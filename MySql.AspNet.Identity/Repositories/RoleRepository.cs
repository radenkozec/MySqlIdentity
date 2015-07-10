using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;

namespace MySql.AspNet.Identity.Repositories
{
    public class RoleRepository<TRole> where TRole: IdentityRole
    {
        private readonly string _connectionString;
        public RoleRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IQueryable<TRole> GetRoles()
        {
            var roles = new List<TRole>();
            using (var conn = new MySqlConnection(_connectionString))
            {
                var reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id,Name FROM aspnetroles", null);

                while (reader.Read())
                {
                    var role = (TRole)Activator.CreateInstance(typeof(TRole));


                    role.Id = reader[0].ToString();
                    role.Name = reader[1].ToString();
                

                    roles.Add(role);
                }

            }
            return roles.AsQueryable();
        }

        public void Insert(IdentityRole role)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                var parameters = new Dictionary<string, object>
                {
                    {"@name", role.Name}, 
                    {"@id", role.Id}
                };

                MySqlHelper.ExecuteNonQuery(conn, @"INSERT INTO aspnetroles (Id, Name) VALUES (@id,@name)", parameters);
            }
        }

        public void Delete(string roleId)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                var parameters = new Dictionary<string, object>
                {
                    {"@id", roleId}
                };

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM aspnetroles WHERE Id = @id", parameters);
            }
        }

        public IdentityRole GetRoleById(string roleId)
        {
            var roleName = GetRoleName(roleId);
            IdentityRole role = null;

            if (roleName != null)
            {
                role = new IdentityRole(roleName, roleId);
            }

            return role;

        }

        private string GetRoleName(string roleId)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                var parameters = new Dictionary<string, object>
                {
                    {"@id", roleId}
                };

                var result = MySqlHelper.ExecuteScalar(conn, CommandType.Text, @"SELECT Name FROM aspnetroles WHERE Id = @id", parameters);
               if (result != null)
               {
                   return result.ToString();
               }
            }
            return null;
        }

        public IdentityRole GetRoleByName(string roleName)
        {
            var roleId = GetRoleId(roleName);
            IdentityRole role = null;

            if (roleId != null)
            {
                role = new IdentityRole(roleName, roleId);
            }

            return role;
        }

        private string GetRoleId(string roleName)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                var parameters = new Dictionary<string, object>()
                {
                    {"@name", roleName}
                };

                var result = MySqlHelper.ExecuteScalar(conn, CommandType.Text, @"SELECT Id FROM aspnetroles WHERE Name = @name", parameters);
                if (result != null)
                {
                    return result.ToString();
                }
            }

            return null;
        }

        public void Update(IdentityRole role)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                var parameters = new Dictionary<string, object>
                {
                    {"@name", role.Name}, 
                    {"@id", role.Id}
                };

                MySqlHelper.ExecuteNonQuery(conn, @"UPDATE aspnetroles SET Name = @name WHERE Id = @id", parameters);
            }
        }


    }
}
