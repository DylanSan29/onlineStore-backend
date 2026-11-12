using Microsoft.EntityFrameworkCore;
using OnlineStoreBackend.Data;
using OnlineStoreBackend.DTOs;
using OnlineStoreBackend.Models;

namespace OnlineStoreBackend.Services;

public sealed class AddressService(OnlineStoreContext context)
{
    public async Task<IReadOnlyList<AddressResponse>> GetForUserAsync(int userId)
    {
        var addresses = await context.Addresses.AsNoTracking().Where(address => address.UserId == userId)
            .OrderByDescending(address => address.IsDefault).ThenBy(address => address.Id).ToListAsync();
        return addresses.Select(ToResponse).ToList();
    }

    public async Task<AddressResponse> CreateAsync(int userId, AddressRequest request)
    {
        var address = new Address { UserId = userId, FullName = "", AddressLine = "", City = "", Country = "" };
        await CopyValuesAsync(address, request);
        context.Addresses.Add(address);
        await context.SaveChangesAsync();
        return ToResponse(address);
    }

    public async Task<bool> UpdateAsync(int userId, int id, AddressRequest request)
    {
        var address = await context.Addresses.FirstOrDefaultAsync(item => item.Id == id && item.UserId == userId);
        if (address is null) return false;
        await CopyValuesAsync(address, request);
        address.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int userId, int id)
    {
        var address = await context.Addresses.FirstOrDefaultAsync(item => item.Id == id && item.UserId == userId);
        if (address is null) return false;
        context.Addresses.Remove(address);
        await context.SaveChangesAsync();
        return true;
    }

    private async Task CopyValuesAsync(Address address, AddressRequest request)
    {
        if (request.IsDefault)
        {
            var previousDefaults = await context.Addresses.Where(item => item.UserId == address.UserId && item.IsDefault).ToListAsync();
            foreach (var previousDefault in previousDefaults)
                previousDefault.IsDefault = false;
        }

        address.FullName = request.FullName.Trim();
        address.AddressLine = request.AddressLine.Trim();
        address.City = request.City.Trim();
        address.State = request.State?.Trim();
        address.PostalCode = request.PostalCode?.Trim();
        address.Country = request.Country.Trim();
        address.IsDefault = request.IsDefault;
    }

    private static AddressResponse ToResponse(Address address) => new()
    {
        Id = address.Id, FullName = address.FullName, AddressLine = address.AddressLine,
        City = address.City, State = address.State, PostalCode = address.PostalCode,
        Country = address.Country, IsDefault = address.IsDefault
    };
}
