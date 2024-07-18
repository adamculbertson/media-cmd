using System.Diagnostics;
using Windows.Media.Control; //Requires setting a target framework for .NET 6 and later
//See: https://learn.microsoft.com/en-us/windows/apps/desktop/modernize/desktop-to-uwp-enhance

namespace MediaCMD
{
    class Program
    {
        public static string[] mediaProcesses = {"AppleMusic", "Firefox"}; //List of process names to search for (find using Task Manager)
        public static string[] appIDs = {"AppleInc.AppleMusicWin_nzyj5cx40ttqa!App", "308046B0AF4A39CB"}; //App IDs to search for (find using commented code below)

        public static bool IsMediaAppRunning() {
            //Determine if any media app is running that is in the mediaProcess list
            // Loop through the media processes
            foreach (string mediaProcess in mediaProcesses)
            {
                Process[] processes = Process.GetProcessesByName(mediaProcess);
                //If the length of the processes list is greater than 0, then this process is running
                //Go ahead and return true
                if(processes.Length > 0) {
                    return true;
                }

            }
            //No processes were found, so return false
            return false;           
        }

        public static GlobalSystemMediaTransportControlsSession? GetPrioritySession(IReadOnlyList<GlobalSystemMediaTransportControlsSession?> sessions) {
            //Determine the priority application to control media for
            //Apps at the beginning of the list have the highest priority
            for(int i = 0; i < appIDs.Length; i++) {
                var test = appIDs[i]; //App ID to test for
                foreach(var session in sessions) {
                    //Not sure if a null session indicates ALL sessions are null or not
                    //Treating it as if that is the case here, though, and end the function if the session is null
                    if(session == null)
                        return null;
                    var appID = session.SourceAppUserModelId; //Get the app ID for the session and compare it to the test value
                    if(appID == test) {
                        return session; //This is the highest priority session
                    }
                }
            }
            return null; //None found, so return null
        }

        //Calling async methods in Main(): https://stackoverflow.com/questions/13002507/how-can-i-call-an-async-method-in-main
        static async Task Main(string[] args)
        {
            string action = "toggle"; //Default to toggle Play/Pause unless an action is specified
            if(args.Length > 0) {
                if(args[0] == "toggle" || args[0] == "play" || args[0] == "pause" || args[0] == "prev" || args[0] == "previous" || args[0] == "next" || args[0] == "stop")
                    action = args[0];
            }
            if(IsMediaAppRunning()) {
                var sessionManager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
                var sessions = sessionManager.GetSessions();

                //The commented code below can be used to find the app IDs of apps that are currently playing media
                //Remove the if statement if you want all connected media apps to be displayed
                /*
                foreach (var session in sessions)
                {
                    var mediaProperties = await session.TryGetMediaPropertiesAsync();
                    var status = session.GetPlaybackInfo().PlaybackStatus.ToString();
                    var appID = session.SourceAppUserModelId;
                    if(status == "Playing") {
                    Console.WriteLine($"{mediaProperties.AlbumArtist} - {mediaProperties.Title}");
                    Console.WriteLine(session.SourceAppUserModelId);
                    }
                    else
                        Console.WriteLine(appID);
                }
                */
                var session = GetPrioritySession(sessions);

                if(session != null) {
                    if(action == "toggle")
                        await session.TryTogglePlayPauseAsync();
                    else if(action == "play")
                        await session.TryPlayAsync();
                    else if(action == "pause")
                        await session.TryPauseAsync();
                    else if(action == "prev" || action == "previous")
                        await session.TrySkipPreviousAsync();
                    else if(action == "next")
                        await session.TrySkipNextAsync();
                    else if(action == "stop")
                        await session.TryStopAsync();
                }
            }
        }
    }
}