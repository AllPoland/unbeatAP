# Current Status
This mod is in early alpha, and not guaranteed to be stable! You can download the latest release [here](https://github.com/AllPoland/unbeatAP/releases/latest).

To keep up to date, join the "UNBEATABLE (PC)" thread in `#future-game-design` in the [Archipelago discord server](https://discord.gg/8Z65BR2).

# What is this?
[Archipelago](https://archipelago.gg/) is a multi-world, multi-game randomizer. That means it takes items from number of different games played by different people, randomizes their locations, and combines them into one big multiplayer randomizer.

This implementation brings UNBEATABLE to Archipelago, allowing you to create/join a lobby alongside any of their [supported games](https://archipelago.gg/games) or [community implementations](https://docs.google.com/spreadsheets/d/1iuzDTOAvdoNe8Ne8i461qGNucg5OuEoF-Ikqs8aUQZw/).

When playing UNBEATABLE Arcade Mode while connected to an Archipelago lobby, you'll have a limited song selection which grows as you or your teammates find your song items, and you'll find items for yourself or your teammates by setting scores and gaining in-game rating.

There's no limit to the number of players or games included in a randomizer. Multiple people can play the same game, and each person can play multiple games if they want to. Archipelago can also generate a singleplayer randomizer if you want to play solo.

# What does this change about the game?
When you're connected to Archipelago, your progress is completely separate - any scores you set, challenges you complete, etc. will not affect the leaderboards or your normal save.

Most songs and characters are locked, and must be unlocked by collecting their respecting song/character items in the multiworld.

The goal of the randomizer is to reach 100 rating. This rating is calculated using a modified version of the vanilla "STAR" rating system, except instead of only taking your top 25 scores, it aggregates all of your scores
in a manner similar to ranking systems in other rhythm games like OSU.

While 100 rating is always the goal, the amount of rating you will earn from each song depends on the difficulty and length options you set for each run. This means that regardless of where your skill level is, or how long you want the randomizer to be, there will be a configuration that works for you.
- More specifically: an internal rating target is calculated based on which songs are included in the randomizer and what scores are deemed "possible" based on your difficulty settings. The internal rating is then scaled so that it appears on a 0-100 scale.
- For generation logic, each song has a different expected score depending on its difficulty level and your settings. This target accuracy is displayed in the UI, so you can easily tell what score the randomizer expects you to earn for each song.

As you gain rating, you will complete checks at regular intervals, which send items to the multiworld. There are plans to add challenge board completions as checks as well as optional alternatives to the rating check system.

### Note:
To get the best experience with this randomizer, it is recommended that you set at least 25 scores in the vanilla game, so you have a good idea of your vanilla rating. Set the "Skill Rating" in the randomizer options a little bit lower than your vanilla rating so that the expected scores match your own skill level.

# Setup
## Prerequisites
- Install the latest version of the [Archipelago Launcher](https://github.com/ArchipelagoMW/Archipelago/releases). You will need this for setting options, generating the world, and using the text client.
- Download [BepInEx Version 5.4 or later](https://github.com/BepInEx/BepInEx/releases) and keep the file handy. You will need this to install the mod.
  - **IMPORTANT:** Make sure you download the 64-bit Windows version of BepInEx, even if you aren't using Windows! It should look like `BepInEx_win_x64_x.x.xx.x.zip`.

## Installing the apworld and configuring
- Download `unbeatable_arcade.apworld` from [the releases page](https://github.com/AllPoland/unbeatAP/releases/latest).

- Open the Archipelago Launcher, and drag `unbeatable_arcade.apworld` onto it. (Alternatively, you can use the "Install APWorld" function in the launcher.) You should see a notification that the apworld was installed.

- **If you know how to edit a YAML file,** use the "Generate Template Options" function in the Archipelago Launcher, find `UNBEATABLE Arcade.yaml` in the folder that opens, and edit your options as needed.

- **If you don't know what a YAML means,** you can alternatively use the "Options Creator" in the "Tool" section. Select UNBEATABLE Arcade from the list and you will see a screen for editing your options. When you are done, click "Export Options" and save the .yaml file somewhere where you can find it later.
  - **Regardless of your method** make sure to read the descriptions of each option carefully! The length and difficulty of this implementation varies greatly if you set your options incorrectly!
    - The "Skill Rating" setting is particularly important! It's recommended that you set at least 25 scores in the vanilla game so you can use your vanilla rating as a reference for this.

## Setting up the multiworld
- **If someone else is hosting the game,** send the organizer your YAML file from the previous step. Also send the [link to the releases page](https://github.com/AllPoland/unbeatAP/releases) so they can download the apworld. You can skip the rest of this section.

- **If you are hosting the game,** use the "Browse Files" function in the Archipelago Launcher, and place your .yaml file into the `Players` folder. Also place the .yaml files from any other players you will be playing with, and install the necessary apworlds for other games if they are not in the [core supported list](https://archipelago.gg/games).

- Use the "Generate" function. This will create a .zip file in the `output` folder.

- Go to [archipelago.gg](https://archipelago.gg) and click "Get Started" > "Host Game". Upload the .zip file from your `output` folder, then click "Create New Room". You now have a room open to connect to!

## Installing the mod and connecting
- Find your game's installation folder; this is most easily done by right-clicking on UNBEATABLE on Steam, and selecting "Manage" > "Browse local files".

- Extract the contents of `BepInEx_win_x64` directly to your game folder.
  - **For Linux users:** BepInEx needs an additional setup step to run through Proton. Instructions for this can be found [here](https://docs.bepinex.dev/articles/advanced/proton_wine.html).

- Download `unbeatAP.zip` from the [releases page](https://github.com/AllPoland/unbeatAP/releases/latest) and extract the contents directly to your game folder.

- If you would like to configure Death Link, trap durations, or item notifications, navigate to `BepInEx/config` and edit `unbeatAP.cfg`. The connection and backup options do not need to be modified here.

- Launch UNBEATABLE, go to Arcade Mode, and a new Archipelago connection UI should appear. Input your room info and click "connect", and the game will reload with Archipelago active!
  - The IP and port are both listed in the game's room page. The IP is everything before the `:`, and the port is the 5-digit number after the `:`. I.e. `archipelago.gg:12345` is IP `archipelago.gg` and port `12345`.
  - Set "slot" to your player name, which you set in your YAML options.
  - If the room has a password, you can also enter it in this UI. If the server has no password, leave the field empty.
 
- To get hints and send Archipelago commands, you need to use the text client included with the Archipelago Launcher for now. An in-game console will be added at a future point.

# Other Info
## Developers
An environment setup and build guide will be added once the mod is in a more stable development cycle.

## Other Repositories
Releases from multiple repositories from me are bundled with this project. The source code for these is listed below.

The source code for the .apworld (randomizer logic) can be found at [this link](https://github.com/AllPoland/Archipelago/tree/main/worlds/unbeatable_arcade).

This project uses my UI Resources repository, which you can find as a submodule in this repository or at [this link](https://github.com/AllPoland/UNBEATABLE-UI-Resources).

# Credits
[@Maya](https://github.com/estradiol-valerate) for help with reverse-engineering and additional programming.

Members of the [UNBEATABLE Modding Community](https://discord.gg/4t8wrZ69Fx), including @zachava and @erik_x for extra reverse-engineering help.

The core [maintainers and contributors of Archipelago](https://github.com/ArchipelagoMW/Archipelago/graphs/contributors), as well as developers in the Archipelago discord for help with creating the apworld.

[@Jarno458](https://github.com/Jarno458) for the [Achipelago.MultiClient.Net library](https://github.com/ArchipelagoMW/Archipelago.MultiClient.Net), which this mod wouldn't be possible without.

@fizzlefree10, @wazaaenor, and others from the Archipelago discord for helping brainstorm ideas.

All the developers and artists at D-CELL GAMES for creating UNBEATABLE.
