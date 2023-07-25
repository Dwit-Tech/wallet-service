using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.WalletService.Core.Events
{
    public interface IEmailEventPublisher
    {
        Task<bool> PublishEmailEventAsync<T>(string topic, T eventData);
    }
}
