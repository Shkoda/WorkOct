::HARDCORE
cd D:\UnityWorkspace\WorkOct\Protocol
d:
::Generate c# source from proto definition
ProtoGen\protogen.exe -i:battle.proto -o:Protocol.cs -ns:WorkOct.Net.Protocol
::-ns:ItKpi.Net.Protocol
::make local copy of protobuf-dll
copy ..\Assets\Lib\protobuf-net.dll protobuf-net.dll
::compile Protocol.cs to Protocol.dll
%WINDIR%\Microsoft.NET\Framework\v2.0.50727\csc /target:library /out:Protocol.dll /nologo /warn:0 /lib:..\Assets\Src\Lib\ /reference:protobuf-net.dll Protocol.cs
::precompile Protocol.dll to PrecompiledSerializer
Precompile\precompile.exe Protocol.dll -o:Precompiled.dll -t:PrecompiledSerializer
::copy result to libs
copy Precompiled.dll ..\Assets\Lib\Precompiled.dll
copy Protocol.dll ..\Assets\Lib\Protocol.dll
::generate types list
ProtobufTypesListGenerator.exe battle.proto ..\Assets\Src\Net\NetworkTypesList.cs WorkOct
::clean up
del *.cs
del *.dll