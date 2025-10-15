using PharmaGo.Domain.Entities;
using PharmaGo.IDataAccess;

namespace PharmaGo.DataAccess.Services
{
    public class DatabaseInitializationService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Role> _roleRepository;

        public DatabaseInitializationService(IRepository<User> userRepository, IRepository<Role> roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        public void Initialize()
        {
            EnsureRolesExist();
            EnsureAdminUserExists();
        }

        private void EnsureRolesExist()
        {
            // Verificar si existen los roles básicos
            var adminRole = _roleRepository.GetOneByExpression(r => r.Name == "Administrator");
            if (adminRole == null)
            {
                var newAdminRole = new Role { Name = "Administrator" };
                _roleRepository.InsertOne(newAdminRole);
            }

            var employeeRole = _roleRepository.GetOneByExpression(r => r.Name == "Employee");
            if (employeeRole == null)
            {
                var newEmployeeRole = new Role { Name = "Employee" };
                _roleRepository.InsertOne(newEmployeeRole);
            }

            var ownerRole = _roleRepository.GetOneByExpression(r => r.Name == "Owner");
            if (ownerRole == null)
            {
                var newOwnerRole = new Role { Name = "Owner" };
                _roleRepository.InsertOne(newOwnerRole);
            }

            _roleRepository.Save();
        }

        private void EnsureAdminUserExists()
        {
            // Verificar si existe al menos un usuario administrador
            var adminRole = _roleRepository.GetOneByExpression(r => r.Name == "Administrator");
            if (adminRole == null)
                throw new InvalidOperationException("Administrator role must exist before creating admin user");

            var adminUser = _userRepository.GetOneDetailByExpression(u => u.Role.Name == "Administrator");
            
            if (adminUser == null)
            {
                // Crear usuario administrador por defecto
                var defaultAdmin = new User
                {
                    UserName = "admin",
                    Email = "admin@pharmago.com",
                    Password = "Admin123!", // En producción, esto debería estar hasheado
                    Address = "Sistema",
                    RegistrationDate = DateTime.Now,
                    Role = adminRole,
                    Pharmacy = null // El admin no pertenece a una farmacia específica
                };

                _userRepository.InsertOne(defaultAdmin);
                _userRepository.Save();
            }
        }
    }
}
