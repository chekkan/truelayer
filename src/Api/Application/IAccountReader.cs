using System;
using System.Threading.Tasks;
using Api.Models;

namespace Api.Application
{
    public interface IAccountReader
    {
        Task<ResourceCollection<Account>> GetAll(Guid userId);
    }
}