﻿using SpikeLib.Responses;
using System;
using System.Text.Json;

namespace SpikeLib.Messages
{
    public interface IMessage
    {
        public static IMessage ParseMessage(JsonDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }
            try
            {

                if (document.RootElement.TryGetProperty(stackalloc byte[] { (byte)'m' }, out var methodProperty))
                {
                    if (methodProperty.ValueKind == JsonValueKind.Number)
                    {
                        switch (methodProperty.GetInt32())
                        {
                            case 1:
                                // Storage response, replied upon successful file upload
                                return new StorageResponse("STATUS", document);
                            case 0:
                                // Port Status
                                return new PortStatusMessage(document);
                            case 2:
                                // Battery [voltage, percentage]
                                return new BatteryMessage(document);
                            case 3:
                                // Button Press
                                break;
                            case 4:
                                // Gesture
                                return new GestureMessage(document);
                            case 12:
                                // Program start [ "prog name", true for start, false for stop
                                return new ProgramStateChangedMessage(document);
                            case 7:
                            case 8:
                                // Initialization?
                                break;
                            case 11:
                                // Program status
                                break;

                        }
                    }
                    else if (methodProperty.ValueKind == JsonValueKind.String)
                    {
                        var methodStr = methodProperty.GetString();
                        if (methodStr == "user_program_error")
                        {
                            return new UserProgramErrorMessage(document);
                        }
                        else if (methodStr == "userProgram.print")
                        {
                            return new UserProgramPrintMessage(document);
                        }
                    }
                    return new UnknownMessage($"Unknown status message:\n{document.RootElement.GetRawText()}");
                }
                else if (document.RootElement.TryGetProperty(stackalloc byte[] { (byte)'i' }, out var idProperty))
                {
                    var idVal = idProperty.GetString()!;
                    char idChar = idVal[0];
                    switch (idChar)
                    {
                        case '1':
                            return new StartWriteProgramResponse(idVal, document);
                        case '2':
                            return new WritePackageResponse(idVal, document);
                    }
                    return new UnknownMessage($"Unknown response message:\n{document.RootElement.GetRawText()}");
                }
                else
                {
                    return new UnknownMessage($"Unknown message type:\n{document.RootElement.GetRawText()}");
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                return new UnknownMessage($"Message Parsing Error:\n{ex}\nMessage:\n{document.RootElement.GetRawText()}");
            }
        }

        string RawText { get; }
    }
}
