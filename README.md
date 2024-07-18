# media-cmd
Control media playback via command line, with a priority.

## Purpose
I do a lot of work in virtual machines and over RDP for my home lab. I get tired of having to leave the virutal machine to control my media. The Elgato Stream Deck's multimedia controls only emulate multimedia key presses, which were always captured by the VM/RDP session. Other solutions I found also did not work. Using the `Open` action on the Stream Deck and pointing it to a batch file to run the executable (so that it can pass parameters) allows me to control multimedia regardless of what application I have running.

## Initial release
This is my first release using C#, so there are a lot of limitations. The primary limitation is that applications are hard-coded into the program to search for. I may or may not fix this, as the application currently meets my needs.

## Applications
Currently, the only way to add applications to the list is to modify the source code and manually get the IDs. There is a block of code commented out that prints a list of all playing applications. Before running this code, modify the `mediaProcesses` list to contain a list of processes that you want to search for, otherwise `IsMediaAppRunning()` will return `false`. The app will print the currently playing media along with its app ID.

In my example, the applications searched for are `Apple Music` and `Firefox`. I'm not sure if the app ID for Firefox will change or not. It's very possible that it might, which means the app ID for it will need to be modified.

## Running
Pass the executable the parameters `play`, `pause`, `toggle`, `stop`, `next`, or `prev`/`previous` to control the detected media players.

## Building
Simply call `dotnet build --configuration=Release` in the project directory, which will put an exe in the `bin/Release` directory.