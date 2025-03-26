using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Сотрудники
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IRepository<Employee> _employeeRepository;

        public EmployeesController(IRepository<Employee> employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        /// <summary>
        /// Получить данные всех сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<EmployeeShortResponse>> GetEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();

            var employeesModelList = employees.Select(x =>
                new EmployeeShortResponse()
                {
                    Id = x.Id,
                    Email = x.Email,
                    FullName = x.FullName,
                }).ToList();

            return employeesModelList;
        }

        /// <summary>
        /// Получить данные сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            var employeeModel = new EmployeeResponse()
            {
                Id = employee.Id,
                Email = employee.Email,
                Roles = employee.Roles.Select(x => new RoleItemResponse()
                {
                    Name = x.Name,
                    Description = x.Description
                }).ToList(),
                FullName = employee.FullName,
                AppliedPromocodesCount = employee.AppliedPromocodesCount
            };

            return employeeModel;
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeResponse>> CreateEmployeeAsync([FromBody] EmployeeRequest request) 
        {
            // from request to entity
            var employee = new Employee()
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email, // TODO: в условиях ДЗ не обозначено, но должно быть уникальным
                Roles = request.Roles.Select(v => new Role() {
                    Name = v.Name,
                    Description = v.Description
                }).ToList(),
                AppliedPromocodesCount = request.AppliedPromocodesCount
            };

            var newEployee = await _employeeRepository.CreateAsync(employee);

            // from entity to response
            var employeeResponce = new EmployeeResponse() {
                Id = newEployee.Id,
                Email = newEployee.Email,
                FullName = newEployee.FullName,
                Roles = newEployee.Roles.Select(x => new RoleItemResponse() {
                    Name = x.Name,
                    Description = x.Description
                }).ToList(),
                AppliedPromocodesCount = newEployee.AppliedPromocodesCount
            };

            return employeeResponce;
        }

        [HttpPut]
        public async Task<ActionResult<EmployeeResponse>> UpdateEmployeeAsync([FromBody] EmployeeRequest request)
        {
            var employee = await _employeeRepository.GetByIdAsync(request.Id);
            if (employee == null) return NotFound();

            employee.FirstName = request.FirstName;
            employee.LastName = request.LastName;
            employee.Email = request.Email;
            employee.Roles = request.Roles.Select(x => new Role() {
                Name = x.Name,
                Description = x.Description
            }).ToList();
            employee.AppliedPromocodesCount = request.AppliedPromocodesCount;

            var updatedEmployee = await _employeeRepository.UpdateAsync(request.Id, employee);

            var employeeResponce = new EmployeeResponse() {
                Id = updatedEmployee.Id,
                Email = updatedEmployee.Email,
                FullName = updatedEmployee.FullName,
                Roles = updatedEmployee.Roles.Select(x => new RoleItemResponse() {
                    Name = x.Name,
                    Description = x.Description
                    }).ToList(),
                AppliedPromocodesCount = updatedEmployee.AppliedPromocodesCount
            };

            return employeeResponce;
        }

        [HttpDelete("{id:Guid}")]
        public async Task<ActionResult> DeleteEmployeeAsync(Guid id) 
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null) return NotFound();

            await _employeeRepository.DeleteAsync(id);

            return NoContent();
        }
    }
}