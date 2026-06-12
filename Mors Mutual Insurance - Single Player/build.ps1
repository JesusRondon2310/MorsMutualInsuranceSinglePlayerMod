# Buscar MSBuild en las rutas comunes de instalación de Visual Studio
$vswhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
$msbuildPath = & $vswhere -latest -requires Microsoft.Component.MSBuild -find MSBuild\Current\Bin\MSBuild.exe

if (-not $msbuildPath) {
   Write-Error "No se encontró MSBuild. Asegúrate de tener Visual Studio o Build Tools instalado."
   exit 1
}

Write-Host "Compilando con: $msbuildPath"

# Compilar la solución
& $msbuildPath "MorsMutualInsuranceMod.sln" /p:Configuration=Release /p:Platform="Any CPU"

if ($LASTEXITCODE -eq 0) {
   Write-Host "Compilación exitosa. El .dll se encuentra en la carpeta 'bin/Release'." -ForegroundColor Green
}
else {
   Write-Host "Error en la compilación. Revisa los mensajes de error." -ForegroundColor Red
}