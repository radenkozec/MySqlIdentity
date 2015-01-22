using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using MySql.AspNet.Identity.Repositories;

namespace MySql.AspNet.Identity
{
    public class MySqlRoleStore<TRole> : IQueryableRoleStore<TRole>
        where TRole : IdentityRole
    {
        private readonly string _connectionString;
        private readonly RoleRepository _roleRepository;
        public MySqlRoleStore()
            : this("DefaultConnection")
        {

        }

        public MySqlRoleStore(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            _roleRepository = new RoleRepository(_connectionString);
        }



        public IQueryable<TRole> Roles
        {
            get { throw new NotImplementedException(); }
        }

        public Task CreateAsync(TRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            _roleRepository.Insert(role);

            return Task.FromResult<object>(null);
        }

        public Task DeleteAsync(TRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("user");
            }

            _roleRepository.Delete(role.Id);

            return Task.FromResult<Object>(null);
        }

        public Task<TRole> FindByIdAsync(string roleId)
        {
            var result = _roleRepository.GetRoleById(roleId) as TRole;

            return Task.FromResult(result);
        }

        public Task<TRole> FindByNameAsync(string roleName)
        {
            var result = _roleRepository.GetRoleByName(roleName) as TRole;
            return Task.FromResult(result);
        }

        public Task UpdateAsync(TRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("user");
            }

            _roleRepository.Update(role);

            return Task.FromResult<Object>(null);
        }

        public void Dispose()
        {
            // connection is automatically disposed
        }
    }
}
