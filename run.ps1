Push-Location -Path "./user"
try {
    dotnet run --project ../src/tgs-cli.csproj -- $args
}
finally {
    Pop-Location
}
