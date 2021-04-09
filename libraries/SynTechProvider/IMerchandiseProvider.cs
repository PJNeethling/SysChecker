using System;

namespace MerchandiseProvider
{
    public interface IMerchandiseProvider : IDisposable
    {
        string Name { get; }

        MerchandiseItems GetItems(string contentDownloadPath);
    }
}
