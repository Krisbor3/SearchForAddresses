using System.Collections.Generic;

namespace SearchForAddresses.Services
{
    public static class HistoryService
    {
        public static Stack<string> SearchedAddresses { get; set; } = new Stack<string>();
    }
}
