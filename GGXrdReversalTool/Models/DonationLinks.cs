using System.Collections.Generic;
using GGXrdReversalTool.Library.Models;

namespace GGXrdReversalTool.Models;

public static class DonationLinks
{
    public static IEnumerable<DonationLink> Items => new List<DonationLink>()
    {
        new DonationLink()
        {
            Name = "PayPal",
            Url = "https://paypal.me/Iquisiquis",
            Icon = "paypal.ico"
        },
        new DonationLink()
        {
            Name = "Ko-fi",
            Url = "https://ko-fi.com/iquis",
            Icon = "ko-fi.png"
        },
        new DonationLink()
        {
            Name = "Patreon",
            Url = "https://www.patreon.com/Iquis",
            Icon = "patreon.png"
        }
    };
}