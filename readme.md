# Slavy’s Mod GTA V Interactive 

### Introduction

This GTA V mod is a custom, locally run HTTP server that listens for POST and GET requests, enabling real-time interaction with your game. Originally designed to work with [TikFinity](https://tikfinity.zerody.one/) to add viewer-driven interactivity for events like gifting, this mod opens up a world of possibilities for customizing your gaming experience. The foundational code is in place, but you’re free to dive into the source and expand it with your own logic — the creative potential is virtually limitless. With minimal upkeep required, I hope this mod serves as a fun and flexible tool for you to build on and enjoy!

## Setup

In order to run this mod you need the following files: 

- [ScriptHookV](http://www.dev-c.com/gtav/scripthookv/)
    - This allows you to run custom scripts on GTA V
- [ScriptHookVDotNet](https://github.com/scripthookvdotnet/scripthookvdotnet/releases)
    - This enables C# compiled code to run within the GTA V process

After installing and following each releases instructions *(drop the following files in the root game directory)* you should have the following files in your game directory e.g `/EpicGames/GTAV/`: 

- dinput8.dll
- NativeTrainer.asi
- ScriptHookV.dll
- ScriptHookVDotNet.asi
- ScriptHookVDotNet.ini
- ScriptHookVDotNet2.dll
- ScriptHookVDotNet3.dll

*This does all of the heavy lifting allowing us to run our mod and many other mods that exist*

Now, download the [latest release](#) and drop the SlavysMod.dll file into your */scripts* folder in the game root directory where GTA 5 is installed. 

**(optional)**: Lastly, I changed the hotkeys for the menus to `F5` and `F8` in the file *ScriptHookVDotNet.ini* for the SHVDN console and refresh hotkeys. To match this, open the file and overwrite the two sections to this: 

``` 
; Specifies the key to immediately reload scripts. Accepts a `Keys` enum name.
ReloadKeyBinding=F8

; Specifies the key to open or close the SHVDN console. Accepts a `Keys` enum name.
ConsoleKeyBinding=F5
```

Your setup is complete! Try launching the game.

> **NOTE:** THIS MOD IS ONLY AVAILABLE FOR SINGLE-PLAYER USAGE. MULTI-PLAYER LAUNCHING WILL LIKELY BE DISABLED 

## Usage

When starting the game, you will know the mod is working correctly if you see the Statistics UI on the top left screen and if the following link brings up a webpage: [http://localhost:6969/](http://localhost:6969/)

Please see the troubleshooting section if that isn’t consistent with what you are experiencing.

## Hotkeys

These hotkeys are used mainly for debugging purposes and to fix the mod on the fly:

- `F4`: Opens a mini-mod menu for some simple and quick mod actions
- `F5`: Opens a console to run your own lines of C# Code to interact with the game
- `F8`: Refreshes the mod
- `Numpad 1`: Deletes the oldest NPC and vehicle NPC you have spawned in
- `Numpad 3`: Resets the character health, effects, ragdoll (Use this if something is wrong with the player or the effects)
- `Numpad 7`: Toggle UI (Shows/Hides the text on the top left)
- `Numpad 9`: Sets the player’s health to 10 (Kills Player)
- `Keyboard E`: While in a vehicle, launches the vehicle forward

## Commands

The following are the endpoints and their description of what they do. You can click on these links to send a GET request and they should apply to your game if it's running correctly. This is what you will put in as the URL for your webhooks.

- [http://localhost:6969/spawn_meleeattacker](http://localhost:6969/spawn_meleeattacker): Spawns a random skinned NPC with a random melee weapon.
- [http://localhost:6969/spawn_armedattacker](http://localhost:6969/spawn_armedattacker): Spawns a random skinned NPC with a random firearm.
- [http://localhost:6969/spawn_gangvehicle](http://localhost:6969/spawn_gangvehicle): Spawns a gang vehicle (Baller) with armed NPCs.
- [http://localhost:6969/spawn_carattack](http://localhost:6969/spawn_carattack): Spawns a vehicle (Caddy) that flies towards the player.
- [http://localhost:6969/spawn_speedboost](http://localhost:6969/spawn_speedboost): Applies a speed boost effect to the player for 10 seconds.
- [http://localhost:6969/spawn_gravity](http://localhost:6969/spawn_gravity): Reverses gravity for the player for 5 seconds.
- [http://localhost:6969/spawn_astro](http://localhost:6969/spawn_astro): Spawns an astronaut-themed NPC equipped with an Up-N-Atomizer weapon.
- [http://localhost:6969/spawn_pirate](http://localhost:6969/spawn_pirate): Spawns an NPC equipped with an RPG, who is resistant to explosions and ragdoll effects.
- [http://localhost:6969/spawn_tank](http://localhost:6969/spawn_tank): Spawns a tank (Rhino) that attacks the player.
- [http://localhost:6969/spawn_planecrash](http://localhost:6969/spawn_planecrash): Teleports player to a plane that crashes into a building instantly killing the player.
- [http://localhost:6969/spawn_juggernaut](http://localhost:6969/spawn_juggernaut): Spawns a heavily armored Juggernaut NPC with a minigun, who is resistant to critical hits, ragdoll effects, and explosions.
- [http://localhost:6969/spawn_zombies](http://localhost:6969/spawn_zombies): Spawns a single zombie NPC equipped with a battle axe.
- [http://localhost:6969/spawn_group](http://localhost:6969/spawn_group): Spawns a group of five armed NPCs with pistols.

## Troubleshooting

There are a few common issues that can be resolved on one’s own. If your issue is not listed or the fix is not working, please let me know in the [issues](https://github.com/yungxslavy/SlavysMod-GTAV-WebHook/issues).

- **Mod Is Not Showing At All**: If the mod isn’t working at all (UI isn’t showing and hotkeys aren’t working), please make sure you have all of the files copied over.

- **Nothing Is Spawning**: If the hotkeys `F4` and `F5` bring up menus but the UI isn’t showing and `Numpad 7` isn’t bringing up the UI either, please ensure you have the `SlavysMod.dll` file inside of the “scripts” folder inside the root game directory “GTAV” like:  `dir\to\GTAV\scripts\SlavysMod.dll`

  When the game loads, press `F8` to reload the script. If that doesn’t fix the issue, please create an [issue](https://github.com/yungxslavy/SlavysMod-GTAV-WebHook/issues).  and I will try providing assistance

- **Endpoints or Webpage Not Working**: If the mod is working and the UI is showing but the endpoints do not spawn in anything, and clicking [http://localhost:6969](http://localhost:6969) fails to bring up the page, please check your firewall settings, as it may be blocking the HTTP server from accepting connections.

- **Updated GTA V & Now Cannot Open Game**: This happens when an update occurs on GTA V and the scripthookv is out of date. You will likely receive a popup message when loading the game that tells you the version is out of date. You can download the most updated version here: [ScriptHookV](http://www.dev-c.com/gtav/scripthookv)

- **Something Else**: If your issue isn’t listed here, please check the log file found in the root GTAV folder listed as “SlavysMod.Log” and try checking that for more insight. Please create an [issue](https://github.com/yungxslavy/SlavysMod-GTAV-WebHook/issues). 
