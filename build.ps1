param(
    [string]
    $project,

    [string]
    $runtime = "linux-musl-x64",
    
    [switch]
    $version=$false,

    [switch]
    $nuget=$false,
    
    [switch]
    $service=$false
)

if(-not $project){
    $project = @(Get-ChildItem src/*.sln)[0].basename
    "project=$project"
}

$ErrorActionPreference = "Stop"

function purgeArtifacts(){
    "--- purge artifacts"
    if(test-path artifacts){
        Remove-Item -Recurse -Force artifacts
    }
}

function getVersion(){
    "--- version vars"
    try{
        nbgv get-version |foreach-object {$_ -replace ": *", "=" -replace " ", "_"} > version.vars
    }catch [System.Management.Automation.CommandNotFoundException]{
        "    nbgv not installed"
        return
    }
    if($LASTEXITCODE -ne 0){ throw "nbgv failed" }
    write-output "PackageVersion=$(nbgv get-version -v "NuGetPackageVersion")" >> version.vars
    if($LASTEXITCODE -ne 0){ throw "nbgv failed" }
    write-output "PackageFullVersion=$(nbgv get-version -v "AssemblyInformationalVersion")" >> version.vars
    if($LASTEXITCODE -ne 0){ throw "nbgv failed" }
    get-content version.vars
}

function build(){
    "--- build/test"
    push-location
    try{
        set-location src
        dotnet restore
        dotnet build -c Release
        if($LASTEXITCODE -ne 0){ throw "build failed" }
        dotnet test -c Release --logger:trx
        if($LASTEXITCODE -ne 0){ throw "tests failed" }
    }finally{
        pop-location
    }
}

function publishService(){
    "--- publish"
    push-location
    try{
        set-location src/$project
        dotnet publish "${project}.csproj" -c Release -o ../../artifacts/app -r $runtime --self-contained false
        if($LASTEXITCODE -ne 0){ throw "publish failed" }
        write-output "dotnet $project.dll" |out-file -Encoding UTF8NoBOM -NoNewline ../../artifacts/app/entrypoint.sh
    }finally{
        pop-location
    }
}

function pushNuget(){
    "--- push nuget"
    if(-not (test-path artifacts/nupkgs)){
        "    no packages found"
        return
    }
    get-item artifacts/nupkgs/*.nupkg | foreach-object {
        "--- push $($_.name)"
        dotnet nuget push $_ -s $env:bamboo_nugetUrl --api-key $env:bamboo_ProGetApiKey
        if($LASTEXITCODE -ne 0){ throw "nuget push failed" }
    }
}

try{
    getVersion
    if($version){
        exit;
    }

    purgeArtifacts
    build

    if($service){
        publishService
    }

    if($nuget){
        pushNuget
    }

    "build succeeded"
}catch{
    "ERROR - $($_.Exception.Message)"
    exit 1
}