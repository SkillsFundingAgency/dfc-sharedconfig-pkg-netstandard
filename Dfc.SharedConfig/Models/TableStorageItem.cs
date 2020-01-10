using Microsoft.WindowsAzure.Storage.Table;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Dfc.SharedConfig.IntegrationTests")]

namespace Dfc.SharedConfig.Models
{
    internal class TableStorageItem : TableEntity
    {
        public string Data { get; set; }
    }
}