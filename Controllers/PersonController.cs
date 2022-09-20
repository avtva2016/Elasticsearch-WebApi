using Microsoft.AspNetCore.Mvc;
using Nest;
using PersonWebApp.Objects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersonWebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PersonController : ControllerBase
    {
        private ConnectionSettings settings;
        private ElasticClient client;

        //Elastic SearchPerson Create, Update, Delete, Exist
        [HttpPost(Name = "ElasticSearchPerson")]
        public async Task<string> CreatePerson(PersonRequest request)
        {
            settings = new ConnectionSettings(new Uri("http://localhost:9220")).DefaultIndex(request.Settings.Index);
            client = new ElasticClient(settings);

            if (Enum.TryParse(typeof(RequestTypes), request.Settings.Method, out object methodId))
            {
                switch (methodId)
                {
                    case RequestTypes.Create:
                        return await CreateAsync(request);
                    case RequestTypes.Update:
                        await UpdateAsync(request);
                        return $"Update Id({request.Settings.Id}) is Success!";
                    case RequestTypes.Delete:
                        await DeleteAsync(request);
                        return $"Delete Index({request.Settings.Index}) is Success!";
                    case RequestTypes.Exist:
                        return await IndexExists(request.Settings.Index);
                }
            }

            return "No action!";
        }

        private IndexResponse CreateIndex(Person person)
        {
            var indexResponse = client.Index(new IndexRequest<Person>(person, IndexName.From<Person>(), 1));
            return indexResponse;
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

            #region Task
            //return await Task.Run(() =>
            //{
            //    var responses = new List<string>();
            //    request.Persons.ForEach(p =>
            //    {
            //        var res = client.IndexDocumentAsync(p);
            //        responses.Add(res.Id.ToString());
            //    });

            //    return $"Created IDs ({string.Join(',', responses)})";
            //});
            #endregion Task
        }

        private async Task<UpdateResponse<Person>> UpdateAsync(PersonRequest request)
        {
            return await client.UpdateAsync<Person>(request.Settings.Id, u => u.Index(request.Settings.Index).Doc(request.Persons[0]));
        }

        private async Task<DeleteIndexResponse> DeleteAsync(PersonRequest request)
        {
            return await client.Indices.DeleteAsync(request.Settings.Index);
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
