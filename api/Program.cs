using System.Text.Json;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.Urls.Add("http://0.0.0.0:8080");

async Task<string> ResolveToken() {
  using var client = new HttpClient();

  // See: https://cloud.google.com/run/docs/container-contract#metadata-server
  client.DefaultRequestHeaders.Add("Metadata-Flavor", "Google");

  var response = await client.GetAsync("http://metadata.google.internal/computeMetadata/v1/instance/service-accounts/default/token");

  var token = await response.Content.ReadAsStringAsync();

  return token;
};

app.MapGet("/resolve", async () => {
  return await ResolveToken();
});

app.MapGet("/invoke", async () => {
  var job = "test-job";
  var projectId = "YOUR_PROJECT_ID_HERE";
  var runtimeRegion = "us-east4";

  var token = await ResolveToken();

  var accessToken = JsonSerializer.Deserialize<Token>(token)!;

  // See: https://cloud.google.com/run/docs/reference/rest/v1/namespaces.jobs/run
  using var client = new HttpClient();

  client.DefaultRequestHeaders.Add(
    HeaderNames.Authorization,
    $"Bearer {accessToken.access_token}"
  );

  await client.PostAsJsonAsync(
    $"https://{runtimeRegion}-run.googleapis.com/apis/run.googleapis.com/v1/namespaces/{projectId}/jobs/{job}:run",
    new RunRequest(
      Overrides: new Overrides(
        ContainerOverrides: new ContainerOverride[] {
          new(
            Name: "",
            Args: new string[] {
              "arg-1",
              "arg-2"
            },
            Env: new EnvVar[] {
              new("ENV_1", "HELLO"),
              new("ENV_2", "WORLD")
            },
            ClearArgs: false
          )
        },
        TaskCount: 1,
        TimeoutSeconds: 10
      )
    ),
    options: new () {
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    }
  );
});

app.Run();

public record Token(
  string access_token
);

public record RunRequest(
  Overrides Overrides
);

public record Overrides(
  ContainerOverride[] ContainerOverrides,
  int TaskCount,
  int TimeoutSeconds
);

public record ContainerOverride(
  string Name,
  string[] Args,
  EnvVar[] Env,
  bool ClearArgs
);

public record EnvVar(
  string Name,
  string Value
);