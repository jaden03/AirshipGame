using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;
using System.Diagnostics;

public class DiscordThing : MonoBehaviour
{
    public Discord.Discord discord;
    public long AppClientId;
    long seconds = (System.DateTimeOffset.Now.ToUnixTimeMilliseconds() / 1000);

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        System.Environment.SetEnvironmentVariable("DISCORD_INSTANCE_ID", "0");
        discord = new Discord.Discord(AppClientId, (System.UInt64)Discord.CreateFlags.NoRequireDiscord);

        UserManager userManager = discord.GetUserManager();

        userManager.OnCurrentUserUpdate += () => {
            var currentUser = userManager.GetCurrentUser();
        };
    }

    void Update()
    {
        if (discord != null)
        {
            discord.RunCallbacks();
        }
    }

    public void UpdateActivity(string state, string details)
    {
        var activityManager = discord.GetActivityManager();

        long time = seconds - ((System.DateTimeOffset.Now.ToUnixTimeMilliseconds() / 1000) - seconds);
        var activity = new Discord.Activity
        {
            State = state,
            Details = details,

            Timestamps =
            {
                Start = time,
            },

            Assets =
            {
                LargeImage = "icon",
                LargeText = "What Lies Above",
            },
            Instance = true,
        };
        activityManager.UpdateActivity(activity, (result) => {
        });
    }

    void OnApplicationQuit()
    {
        var activityManager = discord.GetActivityManager();
        activityManager.ClearActivity((result) => {
        });
        discord.Dispose();
    }
}
