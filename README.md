# STR2WAV
Converts QBASIC PLAY command string to WAV
```                    "STR2WAVE - Convert BASIC play command string into an 8 bit 44100hz mono WAV file.\n\n"+
                    "Usage: STR2WAV [string] [file]\n\n"+
                    "Example:\n"+
                    "STR2WAV T240L8CDEFGAB test.wav"
```

## qbplay.cs
Implements all functionality; including direct Windows audio playback

Check qbplay.cs for latest documention.
```        //--------------------------------------------------------------------------
        //       ACTUAL DIRECT QB PLAY AUDIO IN WINDOWS
        //
        //   basically just call qbplay.PLAY("AB CD etc etc");   anytime u want in ur project
        //   note that by default (like qbasic) its synchronous playback.  call qbplay.PLAY("MB");  OR qbplay.Background=true;   to switch to background audio
        //
        //   ideally, u call qbplay.Init()  and qbplay.Shutdown()  from ur program lifespan, so u dont leak any resources
        //
        //   u can change the volume anytime by setting qbplay.Volume  to  0.0 - 1.0   the default is 0.3
        //
        //   this is a state machine, so any PLAY statements that change tempo, octave, etc.  will remain.
        //   if you need to reset them, u can call qbplay.Defaults()
        //
        //   u can also query qbplay.IsPlaying   and call qbplay.Stop()   if u want finer control over audio playback
        //
        //--------------------------------------------------------------------------
        //       BASIC USAGE WITHOUT BUILT-IN AUDIO
        //
        //   u can directly call qbplay.GenerateWAVFile() OR qbplay.ParseExecuteGenerate()  to just make a *.wav file or generate a PCM sample buffer from text;  does not require audio engine initialization, but state machine properties still apply
        //
        //--------------------------------------------------------------------------
        //       FINAL NOTES
        //   this module is not multithread-safe;  it is intended to be called by main thread only.   internal critical section is only for audio driver callback synchronization
        //
        //--------------------------------------------------------------------------```
