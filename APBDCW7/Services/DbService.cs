using System.Data;

namespace APBDCW7.Services;


using APBDCW7.Models;
using APBDCW7.Models.DTOs;
using APBDCW7.Exceptions;
using Microsoft.Data.SqlClient;

public class DbService(IConfiguration configuration) : IDbService
{
    public async Task<List<ClientTripDTO>> GetClientTripsAsync(int idClient)
    {
        if (! await ClientExistAsync(idClient))
        {
            throw new NotFoundException("Client does not exist");
        }
        
        await using var connection = await GetConnectionAsync();
        
        var sql = "select T.IdTrip, T.Name, T.Description, T.DateFrom, T.DateTo, T.MaxPeople, CT.RegisteredAt, coalesce(CT.PaymentDate,0) FROM Trip T JOIN Client_Trip CT ON T.IdTrip = CT.IdTrip JOIN Client C ON C.IdClient = CT.IdClient WHERE C.IdClient = @idClient";
        
        await using var cmd = new SqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@IdClient", idClient);
        var reader = await cmd.ExecuteReaderAsync();

        var result = new List<ClientTripDTO>();

        while (await reader.ReadAsync())
        {
            result.Add(new ClientTripDTO
            {
                IdTrip = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.GetString(2),
                DateFrom = reader.GetDateTime(3),
                DateTo = reader.GetDateTime(4),
                MaxPeople = reader.GetInt32(5),
                PaymentDate = await reader.IsDBNullAsync(6) ? null : reader.GetInt32(6),
                RegisteredAt = reader.GetInt32(7)
            });
        }

        return result;
    }

    public async Task<List<TripGetDTO>> GetAllTripsAsync()
    {
        
        await using var connection = await GetConnectionAsync();

        var tripToCountries = new Dictionary<int, List<Country>>();

        var countriesSql = "SELECT CT.IdTrip, C.IdCountry, C.Name FROM Country C JOIN dbo.Country_Trip CT ON C.IdCountry = CT.IdCountry";
        
        await using var countriesCmd = new SqlCommand(countriesSql, connection);
        await using var countriesReader = await countriesCmd.ExecuteReaderAsync();
        while (await countriesReader.ReadAsync())
        {
            var idTrip = countriesReader.GetInt32(0);
            var newCountry = new Country
            {
                IdCountry = countriesReader.GetInt32(1),
                Name = countriesReader.GetString(2)
            };
            if (!tripToCountries.TryGetValue(idTrip, out var countries))
            {
                countries = new List<Country>();
            }
            countries.Add(newCountry);
            tripToCountries[idTrip] = countries;
        }
        await countriesReader.CloseAsync();
        
        
        var tripSql = "SELECT IdTrip, Name, Description, DateFrom, DateTo, MaxPeople FROM Trip";
        await using var tripCmd = new SqlCommand(tripSql, connection);
        await using var tripReader = await tripCmd.ExecuteReaderAsync();
        
        var result = new List<TripGetDTO>();
        while (await tripReader.ReadAsync())
        {
            result.Add(new TripGetDTO()
            {
                IdTrip = tripReader.GetInt32(0),
                Name = tripReader.GetString(1),
                Description = tripReader.GetString(2),
                DateFrom = tripReader.GetDateTime(3),
                DateTo = tripReader.GetDateTime(4),
                MaxPeople = tripReader.GetInt32(5),
                Countries = tripToCountries[tripReader.GetInt32(0)]
            });
            
        }
        await tripReader.CloseAsync();
        
        return result;
    }

    public async Task<Client> CreateClientAsync(ClientCreateDTO client)
    {
        await using var connection = await GetConnectionAsync();
        var sql = "INSERT INTO CLIENT(FirstName, LastName, Email, Telephone, Pesel) VALUES (@FirstName, @LastName, @Email, @Telephone, @Pesel);select scope_identity()";
        await using var cmd = new SqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@FirstName", client.FirstName);
        cmd.Parameters.AddWithValue("@LastName", client.LastName);
        cmd.Parameters.AddWithValue("@Email", client.Email);
        cmd.Parameters.AddWithValue("@Telephone", client.Telephone);
        cmd.Parameters.AddWithValue("@Pesel", client.Pesel);

        var newIdClient = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        var newClient = new Client
        {
            IdClient = newIdClient,
            FirstName = client.FirstName,
            LastName = client.LastName,
            Email = client.Email,
            Telephone = client.Telephone,
            Pesel = client.Pesel,
        };
        
        return newClient;

    }

    public async Task RegisterClientToTripAsync(int idClient, int tripId)
    {
        if (! await ClientExistAsync(idClient))
        {
            throw new NotFoundException("Client does not exist");
        }

        if (!await TripExistsAsync(tripId))
        {
            throw new NotFoundException("Trip does not exist");
        }
        
        
        await using var connection = await GetConnectionAsync();
        var sql = "INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt) Values (@IdClient, @IdTrip, @RegisteredAt)";
        await using var cmd = new SqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@IdClient", idClient);
        cmd.Parameters.AddWithValue("@IdTrip", tripId);
        cmd.Parameters.AddWithValue("@RegisteredAt", DateTime.Now.ToString("yyyyMMdd"));
        
        await cmd.ExecuteNonQueryAsync();
        
    }

    public async Task RemoveClientFromTripAsync(int idClient, int tripId)
    {
        await using var connection = await GetConnectionAsync();
        var sql = "DELETE FROM Client_Trip WHERE IdClient = @IdClient AND IdTrip = @IdTrip";
        await using var cmd = new SqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@IdClient", idClient);
        cmd.Parameters.AddWithValue("@IdTrip", tripId);
        
        var rowsAffected = await cmd.ExecuteNonQueryAsync();

        if (rowsAffected == 0)
        {
            throw new NotFoundException("Client to Trip not found");
        }
        
    }

    private async Task<bool> TripExistsAsync(int tripId)
    {
        await using var connecion = await GetConnectionAsync();
        
        var sql = "SELECT IdTrip FROM Trip WHERE IdTrip = @IdTrip";
        await using var cmd = new SqlCommand(sql, connecion);
        cmd.Parameters.AddWithValue("@IdTrip", tripId);
        var result = await cmd.ExecuteReaderAsync();
        
        return result.HasRows;
    }

    private async Task<bool> ClientExistAsync(int idClient)
    {
        await using var connecion = await GetConnectionAsync();
        
        var sql = "SELECT IdClient FROM Client WHERE IdClient = @IdClient";
        await using var cmd = new SqlCommand(sql, connecion);
        cmd.Parameters.AddWithValue("@IdClient", idClient);
        var result = await cmd.ExecuteReaderAsync();
        
        return result.HasRows;
    }


    private async Task<SqlConnection> GetConnectionAsync()
    {
        var connection = new SqlConnection(configuration.GetConnectionString("Default"));

        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }
        
        return connection;
    }
    
}