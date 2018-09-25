using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Piranha.Areas.Manager.Hubs
{
    public class PagePreviewHub : Hub
    {
        public async Task UpdatePage(Guid pageId)
        {
            await Clients.All.SendAsync("UpdatePage", pageId);
        }        
    }
}