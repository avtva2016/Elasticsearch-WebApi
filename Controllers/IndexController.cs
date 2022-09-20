using Microsoft.AspNetCore.Mvc;
using Nest;
using PersonWebApp.Objects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersonWebApp.Controllers
{
    [ApiController]
    public class IndexController : ControllerBase
    {
        private ConnectionSettings settings;
        private ElasticClient client;

        [HttpPut("persons")]
        public async Task<string> CreatePerson(PersonRequest request)
        {
            settings = new ConnectionSettings(new Uri("http://localhost:9220")).DefaultIndex(request.Settings.Index);
            client = new ElasticClient(settings);
            return await CreateAsync(request);
        }

        [HttpPost("persons")]
        public async Task<string> UpdatePerson(PersonRequest request)
        {
            settings = new ConnectionSettings(new Uri("http://localhost:9220")).DefaultIndex(request.Settings.Index);
            client = new ElasticClient(settings);
            await UpdateAsync(request);
            return $"Update Id({request.Settings.Id}) is Success!";
        }

        [HttpDelete("persons")]
        private async Task<string> DeleteAsync(string name)
        {
            await client.Indices.DeleteAsync(name);
            return $"Delete Index({name}) is Success!";
        }

        [HttpGet("index/exist")]
        public async Task<string> CreatePerson(string name)
        {
            settings = new ConnectionSettings(new Uri("http://localhost:9220")).DefaultIndex(name);
            client = new ElasticClient(settings);
            return await IndexExists(name);
        }

        private async Task<string> CreateAsync(PersonRequest request)
        {
            var responses = new List<string>();

            foreach (var p in request.Persons)
            {
                IndexResponse indexResponseAsync = await client.IndexDocumentAsync(p);
                responses.Add(indexResponseAsync.Id.ToString());
            }

            return $"Created IDs ({string.Join(',', responses)})";
        }

        private async Task<UpdateResponse<Person>> UpdateAsync(PersonRequest request)
        {
            return await client.UpdateAsync<Person>(request.Settings.Id, u => u.Index(request.Settings.Index).Doc(request.Persons[0]));
        }

        private async Task<string> IndexExists(string indexName)
        {
            var indexExists = await client.Indices.ExistsAsync(indexName);
            if (indexExists.Exists)
            {
                return $"Index({indexName}) is Exists!";
            }
            else
            {
                return $"Index({indexName}) is not Exists!";
            }
        }
    }
}
