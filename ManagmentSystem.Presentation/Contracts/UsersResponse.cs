using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagmentSystem.Presentation.Contracts
{
    public record UsersResponse(
        Guid id,
        string userName,
        string email);
}
