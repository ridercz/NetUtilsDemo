@ECHO OFF

REM -- Stop the service
NET STOP NetUtilsWorkerService

REM -- Unregister it
SC DELETE NetUtilsWorkerService
