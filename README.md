# SharpShot
Capture screenshots from .NET, using either native Windows APIs or .NET methods.
Screenshots can be saved to disk using a randomly generated file name, or output to the console in base64 encoded form (does not touch disk).

Can be execued using beacons execute-assembly.


## Usage

See the usage instructions:
```
SharpShot.exe /help
```

Capture a full window screenshot using the Windows API and save to disk:
```
SharpShot.exe /outfolder:c:\windows\temp /outformat:img /native
```

Capture a full window screenshot using .NET methods and save to disk:
```
SharpShot.exe /outfolder:c:\windows\temp /outformat:img
```

Capture a full window screenshot using the Windows API and output to the console as Base64:
```
SharpShot.exe /outformat:base64 /native
```

Capture a full window screenshot using .NET methods and output to the console as Base64:
```
SharpShot.exe /outformat:base64
```
## Screenshot

![screenshot](https://github.com/two06/SharpShot/blob/master/Screenshot%20from%202020-03-09%2016-36-29.png)
