﻿using System.Diagnostics;

namespace MagicalKitties.Application.Tests.Integration;

public sealed class SkipIfEnvironmentMissingTheory : TheoryAttribute
{
    public SkipIfEnvironmentMissingTheory()
    {
        if (!IsDockerRunning())
        {
            Skip = "Skipping test as Docker isn't running";
        }
    }

    private static bool IsDockerRunning()
    {
        try
        {
            Process process = new()
                              {
                                  StartInfo = new ProcessStartInfo
                                              {
                                                  FileName = "docker",
                                                  Arguments = "info",
                                                  RedirectStandardOutput = true,
                                                  UseShellExecute = false,
                                                  CreateNoWindow = true
                                              }
                              };

            process.Start();
            process.WaitForExit();
            return process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }
}