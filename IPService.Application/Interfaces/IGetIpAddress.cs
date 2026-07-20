using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Net;
using System.Text;
using IPService.Domain.Entities;

namespace IPService.Application.Interfaces
{
    public interface IGetIpAddress
    {
        /// <summary>
        ///search for an ip from base
        ///input: represantation of ip
        ///output: ip entity and country infos 
        /// </summary>
        Task<(Ipv4Address? IpAddress, Country? Country)> GetByAddressIdAsync(long addressId);
    }
}
