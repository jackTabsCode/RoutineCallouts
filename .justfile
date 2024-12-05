set shell := ["bash", "-c"]

build:
    dotnet build --framework net472 --configuration release

copy:
    mv "obj\\Release\\net472\\RoutineCallouts.dll" "\\Program Files (x86)\\Steam\\steamapps\\common\\Grand Theft Auto V\\plugins\\lspdfr\\"

test:
    just build
    just copy
