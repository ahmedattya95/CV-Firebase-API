using System.Net;
using System.Reactive.Threading.Tasks;
using System.Text.Json.Serialization;

using Firebase.Database;
using Firebase.Database.Query;

using Google.Apis.Util;

namespace ResumeAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateSlimBuilder(args);

            builder.Services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
            });

            var app = builder.Build();
          

            var firebase = new FirebaseClient("https://myresume-aattya-default-rtdb.firebaseio.com");
            var dinos = 
                await firebase.Child("CV").OnceSingleAsync <CV>() ; 
            var todosApi = app.MapGroup("/CV");
            todosApi.MapGet("/", () => dinos);
            todosApi.MapGet("/{id}", (int id) =>
               dinos is not null 
                    ? Results.Ok(dinos)
                    : Results.NotFound());

            app.Run();
        }
    }

    public record CV(string Name, string? Position );

    [JsonSerializable(typeof(CV[]))]
    internal partial class AppJsonSerializerContext : JsonSerializerContext
    {

    }
}
