@ECHO OFF

REM -- The following code will run the service as NT AUTHORITY\LOCAL SYSTEM.
REM -- You do not want to do that in production!!!
SC CREATE NetUtilsWorkerService binPath= "%CD%\TraceWorker.exe"

REM -- Start the newly created service
NET START NetUtilsWorkerService
