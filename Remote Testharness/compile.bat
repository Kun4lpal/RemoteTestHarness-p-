devenv RemoteTestHarness_Project4.sln /rebuild debug
@echo off
cd "Repository\bin\Debug"
start Repository.exe
cd "..\..\..\TServer\bin\Debug"
start TServer.exe
cd "..\..\..\Peer-Comm\bin\Debug"
start Peer-Comm.exe
cd "..\..\..\Client2\bin\Debug"
start Client2.exe

