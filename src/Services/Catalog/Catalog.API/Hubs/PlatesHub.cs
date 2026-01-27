using Microsoft.AspNetCore.SignalR;

namespace Catalog.API.Hubs;

public class PlatesHub : Hub
{
    // Clients call this to subscribe to updates for a specific plate
    public Task JoinPlateGroup(string plateId) => Groups.AddToGroupAsync(Context.ConnectionId, plateId);

    public Task LeavePlateGroup(string plateId) => Groups.RemoveFromGroupAsync(Context.ConnectionId, plateId);
}
