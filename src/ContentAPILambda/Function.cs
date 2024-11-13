using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;

using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using System.Net.Http.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ContentAPILambda;

public class Function
{

    private static readonly HttpClient client = new HttpClient();
    private static async Task<List<Post>> GetPost()
    {
        string api_url = "https://jsonplaceholder.typicode.com/posts/";
        var response = await client.GetAsync(api_url);
        List<Post> posts = new();
        if(response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            posts = JsonSerializer.Deserialize<List<Post>>(jsonString, options);
            
        }
        return posts ?? new List<Post>();
    }

    public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
    {
        if (request.HttpMethod.Equals("GET", StringComparison.CurrentCultureIgnoreCase))
        {
            var posts = await GetPost();
            return new APIGatewayProxyResponse
            {
                Body = JsonSerializer.Serialize(posts),
                StatusCode = 200,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };    
        }
        return new APIGatewayProxyResponse
        {
            StatusCode = 400,
            Body = "Bad Request"
        };
    }
}

public class Post
{
    public int Id { get; set; }
    public int UerId { get; set; }
    public string Title { get; set; } = String.Empty;
    public string Body { get; set; } = String.Empty;
}