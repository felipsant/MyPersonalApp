# Debug
To be able to Debug this project you must first run the project without debug. and then attach to dotnet process. That's why in the program.cs I have the following:
https://stackoverflow.com/questions/67526631/how-can-i-debug-azure-functions-using-net-5-isolated-process-in-visual-studio
```
#if DEBUG
Debugger.Launch();
#endif
```