namespace APBDCW7.Services;

using APBDCW7.Models;
using APBDCW7.Models.DTOs;

public interface IDbService 
{
    public Task<List<ClientTripDTO>> GetClientTripsAsync(int idClient);
    public Task<List<TripGetDTO>> GetAllTripsAsync();
    public Task<Client> CreateClientAsync(ClientCreateDTO client);
    public Task RegisterClientToTripAsync(int idClient, int tripId);
    public Task RemoveClientFromTripAsync(int idClient, int tripId);
}