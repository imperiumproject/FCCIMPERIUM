using SmartHR.Application.DTOs;
using SmartHR.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHR.Application.Interfaces
{
    public interface IMenuItemService
    {
        Task<IEnumerable<MenuItemDTOs>> GetMenuItemsFromD365Async();
    }
}
