using Microsoft.Extensions.Configuration;
using MongoDB.Entities;
using SearchService.Models;
using System;
using System.Net.Http.Json;

namespace SearchService.Services
{
    public class AuctionSvcHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public AuctionSvcHttpClient(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<List<Item>> GetItemsForSearchDb()
        {
            var lastUpdated = await DB.Find<Item, string>()
                .Sort(x => x.Descending(a => a.UpdatedAt))
                .Project(x => x.UpdatedAt.ToString())
                .ExecuteFirstAsync();

            var lastUpdatedString = lastUpdated ?? DateTime.UtcNow.AddYears(-1).ToString("o");
            
            return await _httpClient.GetFromJsonAsync<List<Item>>($"/api/auctions?date={lastUpdatedString}") 
                ?? new List<Item>();
        }
    }
}