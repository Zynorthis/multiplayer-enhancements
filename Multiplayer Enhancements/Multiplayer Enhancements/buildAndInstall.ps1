Write-Host "Generating New DLL..."
dotnet build
Write-Host "Build Complete. Installing Mod..."
Copy-Item "D:\Repositories\multiplayer-enhancements\Multiplayer Enhancements\Multiplayer Enhancements\bin\Debug\netstandard2.0\Multiplayer Enhancements.dll" -Destination "D:\2TB Programs\Steam\steamapps\common\Risk of Rain 2\BepInEx\plugins\"
Write-Host "Done." -ForegroundColor Green