using System;
using System.Diagnostics;

namespace AWSFacade;
public class SQSFacade
{
    public static void PublishMessage(string message)
    {
        ArgumentNullException.ThrowIfNull(message, nameof(message));
        Debug.WriteLine(message);
    }
}
