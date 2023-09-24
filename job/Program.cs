var variables = Environment.GetEnvironmentVariables();

foreach (var key in variables.Keys) {
  Console.WriteLine($"{key} = {variables[key]}");
}

foreach (var arg in args) {
  Console.WriteLine($"  ARG: {arg}");
}