﻿using System.Net.Http.Headers;

namespace OpenCS2hock;

public class OpenShock : Shocker
{
    protected override void ControlInternal(ControlAction action, string shockerId, int intensity, int duration)
    {
        HttpRequestMessage request = new (HttpMethod.Post, $"{Endpoint}/1/shockers/control")
        {
            Headers =
            {
                UserAgent = { new ProductInfoHeaderValue("OpenCS2hock", "1") },
                Accept = { new MediaTypeWithQualityHeaderValue("application/json") },
                Authorization = new AuthenticationHeaderValue("Basic", ApiKey)
            },
            Content = new StringContent(@"[ { "+
                                        $"\"id\": \"{shockerId}\"," +
                                        $"\"type\": {ControlActionToByte(action)},"+
                                        $"\"intensity\": {intensity},"+
                                        $"\"duration\": {duration}"+
                                        "}]")
        };
        this.HttpClient.Send(request);
    }

    private byte ControlActionToByte(ControlAction action)
    {
        return action switch
        {
            ControlAction.Beep => 3,
            ControlAction.Vibrate => 2,
            ControlAction.Shock => 1,
            _ => 0
        };
    }

    public OpenShock(string endpoint, string apiKey, string[] shockerIds, ConfiguredInteger intensity, ConfiguredInteger duration) : base(endpoint, apiKey, shockerIds, intensity, duration)
    {
        
    }
}